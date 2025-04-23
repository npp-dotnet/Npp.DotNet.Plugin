/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Figgle;
using System.Runtime.InteropServices;
using static System.Diagnostics.FileVersionInfo;
using static Npp.DotNet.Plugin.Win32;

namespace Npp.DotNet.Plugin.Demo
{
    /// <summary>
    /// Implements <see cref="IDotNetPlugin"/>.
    /// </summary>
    [GenerateFiggleText("HelloTo", "slant", "Hello, Notepad++ ...")]
    [GenerateFiggleText("HelloFrom", "slant", "from .NET!")]
    partial class Main : IDotNetPlugin
    {
        #region "1. Initialize"
        /// <summary>
        /// Use this to initialize all data your plugin needs when starting up.
        /// At the very least, assign a unique name to the static <see cref="Npp.DotNet.Plugin.PluginData.PluginNamePtr"/> property.
        /// Otherwise <see cref="PluginData.DefaultPluginName"/> will be used.
        /// </summary>
        /// <remarks>
        /// This constructor must be <see langword="static"/> to ensure data is initialized
        /// before the host application calls any unmanaged methods.
        /// </remarks>
        static Main()
        {
            Instance = new Main();
            PluginData.PluginNamePtr = Marshal.StringToHGlobalUni(PluginName);
            Config = new PluginOptions();
            MenuTitles = new PluginMenuTitles();
        }
        #endregion

        #region "2. Implement the plugin interface"
        /// <inheritdoc cref="IDotNetPlugin.OnSetInfo" />
        public void OnSetInfo()
        {
            var sKey = new ShortcutKey(TRUE, FALSE, TRUE, 123); // Ctrl + Shift + F12
            MenuTitles.Load();
            Utils.SetCommand(MenuTitles._0, HelloNpp, sKey);
            Utils.SetCommand(MenuTitles._1, OpenConfigFile);
            Utils.MakeSeparator();
            Utils.SetCommand(MenuTitles._2, DisplayInfo);
        }

        /// <inheritdoc cref="IDotNetPlugin.OnBeNotified" />
        public void OnBeNotified(ScNotification notification)
        {
            if (notification.Header.HwndFrom == PluginData.NppData.NppHandle)
            {
                uint code = notification.Header.Code;
                switch ((NppMsg)code)
                {
                    case NppMsg.NPPN_READY:
                        // do some late-phase initialization
                        Config?.Load();
                        break;
                    case NppMsg.NPPN_TBMODIFICATION:
                        // create your toolbar icon(s)
                        break;
                    case NppMsg.NPPN_FILESAVED:
                        if (string.Compare(Config.FilePath, PluginData.Notepad.GetCurrentFilePath(), StringComparison.InvariantCultureIgnoreCase) == 0)
                            Config?.Load();
                        break;
                    case NppMsg.NPPN_NATIVELANGCHANGED:
                        MenuTitles.Load();
                        I18n.Menu.Localize(new string[] { MenuTitles._0, MenuTitles._1, "-", MenuTitles._2 });
                        break;
                    case NppMsg.NPPN_SHUTDOWN:
                        Config?.Save();
                        // clean up resources
                        PluginData.PluginNamePtr = IntPtr.Zero;
                        PluginData.FuncItems.Dispose();
                        break;
                }
            }
        }

        /// <inheritdoc cref="IDotNetPlugin.OnMessageProc" />
        public NativeBool OnMessageProc(uint msg, UIntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_SIZE:
                    if (wParam == SIZE_MAXIMIZED)
                    {
                        (long height, long width) = ((long)lParam >> 16, (long)lParam & 0xFFFF);
                        uint mbMask = (uint)(MsgBox.ICONASTERISK | MsgBox.OK | MsgBox.TOPMOST);
                        if (NativeLangIsRTL()) mbMask |= (uint)MsgBox.RTLREADING;
                        _ =
                            MsgBoxDialog(
                                PluginData.NppData.NppHandle,
                                $"{MenuTitles._3}: {height}x{width}\0",
                                $"{PluginName}",
                                mbMask
                           );
                    }
                    break;
                default:
                    break;
            }

            return TRUE;
        }
        #endregion

        #region "3. Implement plugin commands"
        /// <summary>
        /// Creates a new buffer and inserts text into it.
        /// </summary>
        static void HelloNpp()
        {
            var editor = PluginData.Editor;
            var eol = editor.LineDelimiter;
            PluginData.Notepad.FileNew();
            editor.SetText(HelloTo.Replace("\r\n", eol));
            editor.AppendText(string.Format("{0}{1}", HelloFrom.Replace("\r\n", eol), eol));
        }

        /// <summary>
        /// Shows the plugin's version number in a system dialog.
        /// </summary>
        static void DisplayInfo()
        {
            uint mbMask = (uint)(MsgBox.ICONQUESTION | MsgBox.OK);
            if (NativeLangIsRTL()) mbMask |= (uint)MsgBox.RTLREADING;
            _ =
                MsgBoxDialog(
                    PluginData.NppData.NppHandle,
                    $"{MenuTitles._4}: {AssemblyVersionString}\0",
                    $"{MenuTitles._5} {PluginName}",
                   mbMask
                );
        }

        /// <summary>
        /// Open the plugin's INI file in Notepad++.
        /// </summary>
        static void OpenConfigFile() => Config?.OpenFile();
        #endregion

        /// <summary><see cref="Main"/> should be a singleton class</summary>
        private Main() { }
        private static bool NativeLangIsRTL()
        {
            string nativeLang = PluginData.Notepad.GetNativeLanguage();
            return new string[] { "arabic", "farsi" }.Any(lang => nativeLang.IndexOf(lang) > -1);
        }
        private static readonly IDotNetPlugin Instance;
        private static readonly PluginOptions Config;
        private static readonly PluginMenuTitles MenuTitles;

        private static string AssemblyVersionString
        {
            get
            {
                string version = "1.0.0.0";
                try
                {
                    string assemblyName = typeof(Main).Namespace!;
                    version =
                        GetVersionInfo(
                            Path.Combine(
                                PluginData.Notepad.GetPluginsHomePath(), assemblyName, $"{assemblyName}.dll")
                            )
                        .FileVersion!;
                }
                catch { }
                return version;
            }
        }

        public static readonly string PluginName = ".NET Demo Plugin\0";
        public static string PluginFolderName => PluginName.Trim(new char[] { '\0', '.' });
    }
}
