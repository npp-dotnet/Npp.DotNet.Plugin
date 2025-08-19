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
        /// <summary>
        /// Initializes a plugin command with an optional keyboard shortcut and initial menu state.
        /// </summary>
        /// <param name="commandName">Command name to display in the plugin menu.</param>
        /// <param name="functionPointer">A delegate of type <see cref="PluginFunc"/>.</param>
        /// <param name="shortcut">A <see cref="ShortcutKey"/>.</param>
        /// <param name="checkOnInit">Whether or not this command's menu item should initially be checked.</param>
        /// <returns>
        /// The new size of the <see cref="PluginData.FuncItems"/> collection; can be used to determine the next available index.
        /// </returns>
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

        /// <summary>
        /// Initializes a plugin command with no keyboard shortcut.
        /// </summary>
        /// <inheritdoc cref="SetCommand(string, PluginFunc, ShortcutKey, bool)"/>
        public static int SetCommand(string commandName, PluginFunc functionPointer)
        {
            return SetCommand(commandName, functionPointer, default, false);
        }

        /// <summary>
        /// Initializes a plugin command with an optional keyboard shortcut.
        /// </summary>
        /// <inheritdoc cref="SetCommand(string, PluginFunc, ShortcutKey, bool)"/>
        public static int SetCommand(string commandName, PluginFunc functionPointer, ShortcutKey shortcut)
        {
            return SetCommand(commandName, functionPointer, shortcut, false);
        }

        /// <summary>
        /// Adds a dividing line to the plugin command menu.
        /// </summary>
        /// <inheritdoc cref="SetCommand(string, PluginFunc, ShortcutKey, bool)"/>
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

        /// <summary>
        /// Gets the window handle of the currently active Scintilla view.
        /// </summary>
        /// <returns>
        /// The window handle of either:
        /// <list type="bullet">
        /// <item><description>the main Scintilla view</description></item>
        /// <item><description>the second Scintilla view</description></item>
        /// </list>
        /// </returns>
        public static IntPtr GetCurrentScintilla()
        {
            Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETCURRENTSCINTILLA, 0U, out int curScintilla);
            return (curScintilla == 0) ? PluginData.NppData.ScintillaMainHandle : PluginData.NppData.ScintillaSecondHandle;
        }

        static readonly Func<IScintillaGateway> GatewayFactory = () => new ScintillaGateway(GetCurrentScintilla());

        public static Func<IScintillaGateway> GetGatewayFactory() => GatewayFactory;
    }
}
