/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Runtime.InteropServices;
using static Npp.DotNet.Plugin.Win32;

namespace Npp.DotNet.Plugin.Demo
{
    /// <summary>
    /// Extends <see cref="DotNetPlugin"/>.
    /// </summary>
    partial class Main : DotNetPlugin
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
        }
        #endregion

        #region "2. Implement the plugin interface"
        /// <inheritdoc cref="IDotNetPlugin.OnSetInfo" />
        public override void OnSetInfo()
        {
            var sKey = new ShortcutKey(TRUE, FALSE, TRUE, 123); // Ctrl + Shift + F12
            Utils.SetCommand("Say \"&Hello\"", HelloNpp, sKey);
            Utils.MakeSeparator();
            Utils.SetCommand("&About", DisplayInfo);
        }

        /// <inheritdoc cref="IDotNetPlugin.OnBeNotified" />
        public override void OnBeNotified(ScNotification notification)
        {
            if (notification.Header.HwndFrom == PluginData.NppData.NppHandle)
            {
                uint code = notification.Header.Code;
                switch ((NppMsg)code)
                {
                    case NppMsg.NPPN_READY:
                        // do some late-phase initialization
                        break;
                    case NppMsg.NPPN_TBMODIFICATION:
                        // create your toolbar icon(s)
                        break;
                    case NppMsg.NPPN_SHUTDOWN:
                        // clean up resources
                        PluginData.PluginNamePtr = IntPtr.Zero;
                        break;
                }
            }
        }

        /// <inheritdoc cref="IDotNetPlugin.OnMessageProc" />
        public override NativeBool OnMessageProc(uint msg, UIntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_SIZE:
                    if (wParam == SIZE_MAXIMIZED)
                    {
                        (long height, long width) = ((long)lParam >> 16, (long)lParam & 0xFFFF);
                        _ =
                            MsgBoxDialog(
                                PluginData.NppData.NppHandle,
                                $"New window size: {height}x{width}\0",
                                $"{PluginName}",
                                (uint)(MsgBox.ICONASTERISK | MsgBox.OK | MsgBox.TOPMOST)
                           );
                    }
                    break;
                default:
                    break;
            }

            return base.OnMessageProc(msg, wParam, lParam);
        }
        #endregion

        #region "3. Implement plugin commands"
        /// <summary>
        /// Creates a new buffer and inserts text into it.
        /// </summary>
        static void HelloNpp()
        {
            NppUtils.Notepad.FileNew();
            NppUtils.Editor.SetText("Hello, Notepad++ ... from .NET!");
        }

        /// <summary>
        /// Shows the plugin's version number in a system dialog.
        /// </summary>
        static void DisplayInfo()
        {
            _ =
                MsgBoxDialog(
                    PluginData.NppData.NppHandle,
                    $"Current version: {NppUtils.AssemblyVersionString}\0",
                    $"About {PluginName}",
                    (uint)(MsgBox.ICONQUESTION | MsgBox.OK)
                );
        }
        #endregion

        /// <summary><see cref="Main"/> should be a singleton class</summary>
        private Main() { }
        private static readonly Main Instance;
        private static readonly string PluginName = ".NET Demo Plugin\0";
    }
}
