/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */
using System.ComponentModel;

namespace Npp.DotNet.Plugin.Demo
{
    class PluginOptions : DefaultSettings
    {
        #region "Examples of possible plugin options"
        [Description("A sample text property (en-US)"), Category("Strings")]
        public string DefaultMessage { get; set; } = "Hello, World!";

        [Description("A sample text property (ja-JP)"), Category("Strings")]
        public string LocalizedMessage { get; set; } = "こんにちは、皆さん‼";

        [Description("A sample enum property"), Category("Enum")]
        public ConsoleColor Color { get; set; } = ConsoleColor.Green;

        [Description("A sample Boolean property"), Category("Boolean")]
        public bool AutoClose { get; set; } = false;

        [Description("A sample integer property"), Category("Integer")]
        public int MarginWidth { get; set; } = 16;

        [Description("A sample floating point property"), Category("Double")]
        public double Scale { get; set; } = 2.5;
        #endregion

        public string FilePath
        {
            get
            {
                var folderName = Main.PluginName.Trim(new char[] { '\0', ' ' });
                var configDir = new DirectoryInfo(Path.Combine(NppUtils.ConfigDirectory, folderName));
                if (!configDir.Exists)
                    configDir = Directory.CreateDirectory(Path.Combine(NppUtils.ConfigDirectory, folderName));
                return Path.Combine(configDir.FullName, "settings.example.ini");
            }
        }

        public void Load() => base.Load(FilePath);
        public void Save() => base.Save(FilePath);
        public override void OpenFile() => NppUtils.Notepad.OpenFile(FilePath);
    }
}
