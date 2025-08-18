/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *                         2024 Robert Di Pardo <dipardo.r@gmail.com>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;

namespace Npp.DotNet.Plugin
{
    public static class Utils
    {
        public static int SetCommand(string commandName, PluginFunc functionPointer, ShortcutKey shortcut, bool checkOnInit)
        {
            FuncItem funcItem = default;
            funcItem.CmdID = PluginData.FuncItems.Items.Count;
            funcItem.ItemName = commandName;
            if (functionPointer != null)
                funcItem.PFunc = new PluginFunc(functionPointer);
            if (shortcut.Key != 0)
                funcItem.PShKey = shortcut;
            _ = Enum.TryParse<NativeBool>($"{checkOnInit}", true, out funcItem.Init2Check);
            PluginData.FuncItems.Add(funcItem);
            return PluginData.FuncItems.Items.Count;
        }

        public static int SetCommand(string commandName, PluginFunc functionPointer)
        {
            return SetCommand(commandName, functionPointer, default, false);
        }

        public static int SetCommand(string commandName, PluginFunc functionPointer, ShortcutKey shortcut)
        {
            return SetCommand(commandName, functionPointer, shortcut, false);
        }

        public static int MakeSeparator()
        {
            return SetCommand("-", null);
        }

        /// <summary>
        /// Sets the menu state of the plugin command associated with <paramref name="funcId"/>, if any.
        /// </summary>
        /// <param name="funcId">Index within <see cref="PluginData.FuncItems"/> of the menu item to check/uncheck.</param>
        /// <param name="isChecked">Whether the menu item should be checked or not.</param>
        public static void CheckMenuItem(int funcId, bool isChecked)
        {
            try
            {
                _ = Win32.SendMessage(PluginData.NppData.NppHandle,
                    (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                    (uint)(PluginData.FuncItems?.Items?[funcId].CmdID).GetValueOrDefault(),
                    isChecked ? 1 : 0);
            }
            finally { }
        }

        /// <summary>
        /// If a menu item (in your plugin's drop-down menu) has a checkmark:
        /// <list type="bullet">
        /// <item><description>if it's checked, uncheck it</description></item>
        /// <item><description>if it's unchecked, check it</description></item>
        /// </list>
        /// </summary>
        /// <param name="funcId">Index within <see cref="PluginData.FuncItems"/> of the menu item to check/uncheck.</param>
        /// <param name="isCurrentlyChecked">Whether the menu item should be checked or not.</param>
        public static void CheckMenuItemToggle(int funcId, ref bool isCurrentlyChecked)
        {
            // toggle value
            isCurrentlyChecked = !isCurrentlyChecked;
            CheckMenuItem(funcId, isCurrentlyChecked);
        }

        public static IntPtr GetCurrentScintilla()
        {
            Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETCURRENTSCINTILLA, 0U, out int curScintilla);
            return (curScintilla == 0) ? PluginData.NppData.ScintillaMainHandle : PluginData.NppData.ScintillaSecondHandle;
        }

        static readonly Func<IScintillaGateway> GatewayFactory = () => new ScintillaGateway(GetCurrentScintilla());

        public static Func<IScintillaGateway> GetGatewayFactory() => GatewayFactory;
    }
}
