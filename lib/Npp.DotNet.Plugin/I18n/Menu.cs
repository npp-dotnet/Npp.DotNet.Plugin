/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Runtime.InteropServices;
using static Npp.DotNet.Plugin.Win32;

namespace Npp.DotNet.Plugin.I18n
{
    /// <summary>
    /// Utilities for localizing plugin command menus.
    /// </summary>
    public static class Menu
    {
        /// <summary>
        /// Changes the text of a menu item, located by menu ID.
        /// </summary>
        /// <param name="menuItemID">The ID of a menu item, e.g., the <see cref="FuncItem.CmdID"/> field of a <see cref="FuncItem"/>.</param>
        /// <param name="text">The string to be displayed on the menu item.</param>
        /// <returns><see langword="true"/> if the menu item text could be changed, otherwise <see langword="false"/>.</returns>
        public static bool SetMenuItemText(int menuItemID, string text)
        {
            bool result = false;
            MenuItemInfo mii = default;
            mii.cbSize = Marshal.SizeOf<MenuItemInfo>();
            mii.fMask = MIIM_STATE | MIIM_STRING;
            mii.dwTypeData = IntPtr.Zero;
            IntPtr hMenu = SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETMENUHANDLE, (uint)NppMsg.NPPPLUGINMENU);

            if (GetMenuItemInfo(hMenu, menuItemID, false, ref mii))
            {
                mii.cch = (mii.cch + 1) * Marshal.SizeOf<char>();
                mii.dwTypeData = Marshal.AllocHGlobal(mii.cch);
                GetMenuItemInfo(hMenu, menuItemID, false, ref mii);
                string currentTitle = Marshal.PtrToStringUni(mii.dwTypeData) ?? string.Empty;
                int sKeyPos = currentTitle.LastIndexOf('\t');
                string shortcut = (sKeyPos > -1) ? currentTitle.Substring(sKeyPos) : string.Empty;
                Marshal.FreeHGlobal(mii.dwTypeData);
                mii.dwTypeData = IntPtr.Zero;
                mii.cch = (text.Length + 1) * Marshal.SizeOf<char>();
                mii.dwTypeData = Marshal.StringToHGlobalUni(text + shortcut);
                result = SetMenuItemInfo(hMenu, menuItemID, false, mii);
                Marshal.FreeHGlobal(mii.dwTypeData);
            }

            return result;
        }

        /// <summary>
        /// Adds the translations in <paramref name="menuTitles"/> to the command menu of the active plugin, located by name.
        /// </summary>
        /// <param name="menuTitles">An ordered list of plugin command menu titles.</param>
        public static void Localize(string[] menuTitles)
        {
            MenuItemInfo mii = default;
            mii.cbSize = Marshal.SizeOf<MenuItemInfo>();
            mii.fMask = MIIM_STATE | MIIM_ID | MIIM_SUBMENU | MIIM_STRING;
            mii.dwTypeData = IntPtr.Zero;
            IntPtr hMenu = SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETMENUHANDLE, (uint)NppMsg.NPPPLUGINMENU);

            for (int menuPos = 0; GetMenuItemInfo(hMenu, menuPos, true, ref mii); menuPos++)
            {
                mii.cch = (mii.cch + 1) * Marshal.SizeOf<char>();
                mii.dwTypeData = Marshal.AllocHGlobal(mii.cch);
                _ = GetMenuItemInfo(hMenu, menuPos, true, ref mii);

                string currentTitle = Marshal.PtrToStringUni(mii.dwTypeData) ?? string.Empty;
                Marshal.FreeHGlobal(mii.dwTypeData);
                mii.dwTypeData = IntPtr.Zero;

                if (mii.hSubMenu != IntPtr.Zero &&
                    string.Compare(currentTitle, Marshal.PtrToStringUni(PluginData.PluginNamePtr), StringComparison.Ordinal) == 0)
                {
                    hMenu = mii.hSubMenu;
                    for (int itemPos = 0; GetMenuItemInfo(hMenu, itemPos, true, ref mii); itemPos++)
                    {
                        if (mii.wID != 0)
                        {
                            mii.cch = (mii.cch + 1) * Marshal.SizeOf<char>();
                            mii.dwTypeData = Marshal.AllocHGlobal(mii.cch);
                            _ = GetMenuItemInfo(hMenu, itemPos, true, ref mii);

                            currentTitle = Marshal.PtrToStringUni(mii.dwTypeData) ?? string.Empty;
                            int sKeyPos = currentTitle.LastIndexOf('\t');
                            string shortcut = (sKeyPos > -1) ? currentTitle.Substring(sKeyPos) : string.Empty;
                            Marshal.FreeHGlobal(mii.dwTypeData);

                            mii.cch = (menuTitles[itemPos].Length + 1) * Marshal.SizeOf<char>();
                            mii.dwTypeData = Marshal.StringToHGlobalUni(menuTitles[itemPos] + shortcut);
                            _ = SetMenuItemInfo(hMenu, itemPos, true, mii);

                            Marshal.FreeHGlobal(mii.dwTypeData);
                            mii.dwTypeData = IntPtr.Zero;
                        }
                    }
                }
            }
        }
    }
}
