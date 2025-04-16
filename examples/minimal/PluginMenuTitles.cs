/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */
using Npp.DotNet.Plugin.Extensions;

namespace Npp.DotNet.Plugin.Demo
{
    class PluginMenuTitles : DefaultSettings
    {
        #region "Localized menu titles"
        public string _0 { get; set; } = "";
        public string _1 { get; set; } = "";
        public string _2 { get; set; } = "";
        public string _3 { get; set; } = "";
        public string _4 { get; set; } = "";
        public string _5 { get; set; } = "";
        #endregion

        public PluginMenuTitles() => SetDefaultMessages();

        public void Load()
        {
            if (!File.Exists(LocalizationPath))
            {
                SetDefaultMessages();
                return;
            }
            base.Load(LocalizationPath);
        }

        private void SetDefaultMessages()
        {
            _0 = @"Say ""&Hello""";
            _1 = "Plugin &settings";
            _2 = "&About...";
            _3 = "New window size";
            _4 = "Current version";
            _5 = "About";
        }

        private static string LocalizationPath
        {
            get
            {
                var localeDir =
                    new DirectoryInfo(
                        Path.Combine(PluginData.Notepad.GetConfigDirectory(), Main.PluginFolderName, "localizations"));

                var assetDir =
                    new DirectoryInfo(
                        Path.Combine(PluginData.Notepad.GetPluginsHomePath(), typeof(Main).Namespace!, "localizations"));

                if (!localeDir.Exists)
                {
                    localeDir = Directory.CreateDirectory(localeDir.FullName);
                    if (assetDir.Exists)
                    {
                        foreach (var locale in assetDir.GetFiles())
                            locale.CopyTo(Path.Combine(localeDir.FullName, locale.Name));
                    }
                }

                string localeFile = Path.Combine(localeDir.FullName, $"{PluginData.Notepad.GetNativeLanguage()}.ini");
                string assetFile = Path.Combine(assetDir.FullName, Path.GetFileName(localeFile));
                if (!File.Exists(localeFile) && File.Exists(assetFile))
                    File.Copy(assetFile, localeFile);

                return localeFile;
            }
        }
    }
}
