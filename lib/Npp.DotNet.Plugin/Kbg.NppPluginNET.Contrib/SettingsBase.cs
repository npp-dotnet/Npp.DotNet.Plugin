/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *                         2017-2020 Joakim Wennergren <joakim.wennergren@neovici.se>
 *                         2020-2024 Bas de Reuver ("BdR76") <bdr1976@gmail.com>
 * SPDX-License-Identifier: Apache-2.0 OR LicenseRef-CsvQuery OR GPL-3.0-or-later
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Npp.DotNet.Plugin
{
    /// <summary>
    /// Utilities for storing, viewing, and updating the settings of the plugin.<br/>
    /// Extracted from <a href="https://github.com/BdR76/CSVLint/blob/master/CSVLintNppPlugin/PluginInfrastructure/SettingsBase.cs">CSVLint</a>.
    /// </summary>
#if NET7_0_OR_GREATER
    // https://learn.microsoft.com/dotnet/core/deploying/trimming/fixing-warnings#functionality-with-requirements-on-its-input
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
    public class SettingsBase
    {
        protected readonly INotepadPPGateway _npp;
        protected string _iniFilePath;
        protected string _pluginName;

        public SettingsBase()
        {
            _pluginName = Marshal.PtrToStringUni(PluginData.PszPluginName) ?? PluginData.DefaultPluginName;
            _npp = new NotepadPPGateway();
            _iniFilePath = Path.Combine(_npp.GetPluginConfigPath(), $"{_pluginName}.ini");
        }

        /// <summary>
        /// By default loads settings from the default N++ config folder
        /// </summary>
        /// <param name="loadFromFile"> If false will not load anything and have default values set </param>
        public SettingsBase(bool loadFromFile = true) : this()
        {
            // Set defaults
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault() is DefaultValueAttribute def)
                {
                    propertyInfo.SetValue(this, def.Value, null);
                }
            }
            if (loadFromFile)
                ReadFromIniFile();
        }

        /// <summary>
        /// Reads all (existing) settings from an ini-file
        /// </summary>
        /// <param name="filename">File to write to (default is N++ plugin config)</param>
        public void ReadFromIniFile(string filename = null)
        {
            filename = filename ?? _iniFilePath;
            if (!File.Exists(filename)) return;

            // Load all sections from file
            var loaded = GetType().GetProperties()
                .Select(x => ((CategoryAttribute)x.GetCustomAttributes(typeof(CategoryAttribute), false).FirstOrDefault())?.Category ?? "General")
                .Distinct()
                .ToDictionary(section => section, section => GetKeys(filename, section));

            foreach (var propertyInfo in GetType().GetProperties().Where(x => x.CanWrite))
            {
                var category = ((CategoryAttribute)propertyInfo.GetCustomAttributes(typeof(CategoryAttribute), false).FirstOrDefault())?.Category ?? "General";
                var name = propertyInfo.Name;
                if (loaded.TryGetValue(category, out Dictionary<string, string> value) && value.TryGetValue(name, out string rawString) && !string.IsNullOrEmpty(rawString))
                {
                    try
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                            propertyInfo.SetValue(this, ParseEnumProperty(propertyInfo.PropertyType, rawString), null);
                        else if (int.TryParse(rawString, out int intVal))
                        {
                            // Parse 0 as false, !0 as true
                            if (propertyInfo.PropertyType == typeof(bool))
                                propertyInfo.SetValue(this, intVal != 0, null);
                            else
                                propertyInfo.SetValue(this, intVal, null);
                        }
                        else if (double.TryParse(rawString, out double dblVal))
                            propertyInfo.SetValue(this, dblVal, null);
                        else if (bool.TryParse(rawString, out bool boolVal))
                            propertyInfo.SetValue(this, boolVal, null);
                        else
                            propertyInfo.SetValue(this, rawString, null);
                    }
                    catch (Exception e)
                    {
                        _ = Win32.MsgBoxDialog(
                                IntPtr.Zero,
                                $"Setting property {propertyInfo.Name}=\"{rawString}\" failed with message:\r\n\r\n{e.Message}\0",
                                $"{e.GetType().Name}\0",
                                (uint)Win32.MsgBox.ICONERROR);
                    }
                }
            }
        }

        /// <summary>
        /// Saves all settings to an ini-file, under "General" section
        /// </summary>
        /// <param name="filename">File to write to (default is N++ plugin config)</param>
        public void SaveToIniFile(string filename = null)
        {
            filename = filename ?? _iniFilePath;
            NppUtils.CreateConfigSubDirectoryIfNotExists();

            // Win32.WritePrivateProfileSection (that NppPlugin uses) doesn't work well with non-ASCII characters. So we roll our own.
            using (var fp = new StreamWriter(filename, false, Encoding.UTF8))
            {
                fp.WriteLine("; {0} settings file", _pluginName);

                foreach (var section in GetType()
                    .GetProperties()
                    .GroupBy(x => ((CategoryAttribute)x.GetCustomAttributes(typeof(CategoryAttribute), false).FirstOrDefault())?.Category ?? "General"))
                {
                    fp.WriteLine(Environment.NewLine + "[{0}]", section.Key);
                    foreach (var propertyInfo in section.Where(x => x.CanWrite).OrderBy(x => x.Name))
                    {
                        if (propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute description)
                            fp.WriteLine("; " + description.Description.Replace(Environment.NewLine, Environment.NewLine + "; "));
                        fp.WriteLine("{0}={1}", propertyInfo.Name, propertyInfo.GetValue(this, null));
                    }
                }
            }
        }

        /// <summary>
        /// Parses a named enum value, ignoring case, or the underlying numeric value.
        /// </summary>
        /// <param name="TEnum">Some enum type.</param>
        /// <param name="propertyStr">A name defined by <paramref name="TEnum"/>, or a number.</param>
        /// <returns>
        /// For example, if <paramref name="TEnum"/> is <see cref="Win32.MsgBox"/>, the result is <see cref="Win32.MsgBox.OK"/> when
        /// <paramref name="propertyStr"/> is <c>"Ok"</c> or <c>"0"</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="propertyStr"/> is not a value of <paramref name="TEnum"/>.
        /// </exception>
        private static object ParseEnumProperty(Type TEnum, string propertyStr)
        {
            string valueStr =
#if NETCOREAPP
                Enum.TryParse(TEnum, propertyStr, true, out object _)
                ? propertyStr
                : uint.TryParse(propertyStr, out uint intVal) && Enum.TryParse(TEnum, $"{intVal}", false, out _)
#else // .NET Framework
                (Enum.GetNames(TEnum)?.Any(n => string.Compare(n, propertyStr, StringComparison.OrdinalIgnoreCase) == 0)).GetValueOrDefault(false)
                ? propertyStr
                : uint.TryParse(propertyStr, out uint intVal)
#endif
                    ? $"{intVal}"
                    : throw new ArgumentException($"Invalid value \"{propertyStr}\" for enum {TEnum.FullName}");

            return Enum.Parse(TEnum, valueStr, true);
        }

        /// <summary>
        /// Read a section from an ini-file
        /// </summary>
        /// <param name="iniFile">Path to ini-file</param>
        /// <param name="category">Section to read</param>
        protected Dictionary<string, string> GetKeys(string iniFile, string category)
        {
            var buffer = new byte[8 * 1024];

            _ = Win32.GetPrivateProfileSection(category, buffer, buffer.Length, iniFile);
            var tmp = Encoding.UTF8.GetString(buffer).Trim('\0').Split('\0');
            return tmp.Select(x => x.Split(new[] { '=' }, 2))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0], x => x[1]);
        }

        /// <summary> Opens the config file directly in Notepad++ </summary>
        public void OpenFile()
        {
            if (!File.Exists(_iniFilePath)) SaveToIniFile();
            Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_DOOPEN, 0U, _iniFilePath);
        }
    }
}
