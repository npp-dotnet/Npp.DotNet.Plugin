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
        /// if a menu item (for your plugin's drop-down menu) has a checkmark, check/uncheck it, and call its associated funcId.
        /// </summary>
        /// <param name="funcId">the index of the menu item of interest</param>
        /// <param name="isChecked">whether the menu item should be checked</param>
        public static void CheckMenuItem(int funcId, bool isChecked)
        {
            _ = Win32.CheckMenuItem(
                Win32.GetMenu(PluginData.NppData.NppHandle),
                (PluginData.FuncItems?.Items?[funcId].CmdID).GetValueOrDefault(),
                Win32.MF_BYCOMMAND | (isChecked ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
        }

        /// <summary>
        /// if a menu item (for your plugin's drop-down menu) has a checkmark:<br></br>
        /// - if it's checked, uncheck it<br></br>
        /// - if it's unchecked, check it.
        /// Either way, call its associated funcId.
        /// </summary>
        /// <param name="funcId">the index of the menu item of interest</param>
        /// <param name="isCurrentlyChecked">whether the menu item is currently checked</param>
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
