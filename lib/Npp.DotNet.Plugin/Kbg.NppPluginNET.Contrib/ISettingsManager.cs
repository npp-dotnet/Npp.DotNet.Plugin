/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

#if NET7_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Npp.DotNet.Plugin
{
	/// <summary>
	/// Provides plugins with methods to read and write INI configuration files.
	/// </summary>
	public interface ISettingsManager
	{
		void Load(string filePath = null);
		void Save(string filePath = null);
		void OpenFile();
	}

	/// <summary>
	/// Default implementation of the <see cref="ISettingsManager"/> interface.
	/// </summary>
#if NET7_0_OR_GREATER
	// https://learn.microsoft.com/dotnet/core/deploying/trimming/fixing-warnings#functionality-with-requirements-on-its-input
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
	public class DefaultSettings : ISettingsManager
	{
		/// <summary>
		/// Token used to separate property keys from values.
		/// </summary>
		public char KeyValueSeparator = '=';

		/// <summary>
		/// Sets all writable properties of this <see cref="ISettingsManager"/> from the values in the INI file at <paramref name="filePath"/>.
		/// </summary>
		/// <param name="filePath">Full file path of an INI file.</param>
		public virtual void Load(string filePath = null)
		{
			if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
				return;

			var keyValuePairs = new Dictionary<string, string>();
			foreach (string line in File.ReadAllLines(filePath))
			{
				try
				{
					if (line.Contains(KeyValueSeparator))
					{
						var kv = line.Split(new char[] { KeyValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
						if (kv.Length > 1)
							keyValuePairs.Add(kv.First()?.Trim(), string.Join("", kv.Skip(1)).Trim());
					}
				}
				catch (Exception e)
				{
					_ = Win32.MsgBoxDialog(
							IntPtr.Zero,
							$"Parsing \"{line}\" failed with message:\r\n\r\n{e.Message}\0",
							$"{e.GetType().Name}\0",
							(uint)Win32.MsgBox.ICONERROR);
				}
			}

			foreach (var keyValueStr in keyValuePairs)
			{
				string key = keyValueStr.Key;
				string valueStr = keyValueStr.Value;

				try
				{
					var propInfo = GetType().GetProperties().FirstOrDefault(p => p.CanWrite && p.Name == key);
					if (propInfo is null)
						continue;

					if (propInfo.PropertyType.IsEnum)
						propInfo.SetValue(this, ParseEnumProperty(propInfo.PropertyType, valueStr), null);
					else if (int.TryParse(valueStr, out int intVal))
					{
						// Parse 0 as false, !0 as true
						if (propInfo.PropertyType == typeof(bool))
							propInfo.SetValue(this, intVal != 0, null);
						else
							propInfo.SetValue(this, intVal, null);
					}
					else if (double.TryParse(valueStr, out double dblVal))
						propInfo.SetValue(this, dblVal, null);
					else if (bool.TryParse(valueStr, out bool boolVal))
						propInfo.SetValue(this, boolVal, null);
					else
						propInfo.SetValue(this, valueStr, null);

				}
				catch (Exception e)
				{
					_ = Win32.MsgBoxDialog(
							IntPtr.Zero,
							$"Setting property {key}{KeyValueSeparator}\"{valueStr}\" failed with message:\r\n\r\n{e.Message}\0",
							$"{e.GetType().Name}\0",
							(uint)Win32.MsgBox.ICONERROR);
				}
			}

		}

		/// <summary>
		/// Dumps all writable properties of this <see cref="ISettingsManager"/> to the INI file at <paramref name="filePath"/>.
		/// </summary>
		/// <param name="filePath">Full file path of an INI file.</param>
		public virtual void Save(string filePath = null)
		{
			if (string.IsNullOrEmpty(filePath) || GetType().GetProperties().Length < 1)
				return;

			try
			{
				var iniText = new StringBuilder();
				var props =
					GetType().GetProperties().Where(p => p.CanWrite)
					.GroupBy(p => p.GetCustomAttribute<CategoryAttribute>() ?? new CategoryAttribute("Misc"));
				foreach (var section in props.Select(s => s.Key).Distinct())
				{
					iniText.AppendLine($"[{section.Category}]");
					foreach (var propGrp in props.Where(g => g.Key.Category == section.Category))
					{
						foreach (var propInfo in propGrp)
						{
							if (!(propInfo.GetCustomAttribute<DescriptionAttribute>() is null))
								iniText.AppendLine($"; {propInfo.GetCustomAttribute<DescriptionAttribute>().Description}");

							var propValue = propInfo.PropertyType.IsEnum
								? ParseEnumProperty(propInfo.PropertyType, $"{propInfo.GetValue(this, null)}")
								: propInfo.GetValue(this, null);

							iniText.AppendLine($"{propInfo.Name}{KeyValueSeparator}{propValue}");
						}
					}
					iniText.AppendLine();
				}

				File.WriteAllText(filePath, iniText.ToString());
			}
			catch (Exception e)
			{
				_ = Win32.MsgBoxDialog(
						IntPtr.Zero,
						$"Saving properties failed with message:\r\n\r\n{e.StackTrace}\0",
						$"{e.GetType().Name}\0",
						(uint)Win32.MsgBox.ICONERROR);
			}
		}

		/// <summary>
		/// Opens the INI file of this <see cref="ISettingsManager"/>.
		/// </summary>
		/// <remarks>Deriving classes must implement this method for themselves.</remarks>
		public virtual void OpenFile() { }

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
		protected static object ParseEnumProperty(Type TEnum, string propertyStr)
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
	}
}
