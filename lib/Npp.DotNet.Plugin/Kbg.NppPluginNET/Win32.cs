/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *                         2024 Robert Di Pardo <dipardo.r@gmail.com>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Npp.DotNet.Plugin
{
    public class Win32
    {
        public static readonly IntPtr NULL = IntPtr.Zero;
        public static readonly NativeBool FALSE = NativeBool.False;
        public static readonly NativeBool TRUE = NativeBool.True;

        /// <summary>
        /// Get the scroll information of a scroll bar or window with scroll bar<br/>
        /// </summary>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/windows/win32/api/winuser/ns-winuser-scrollinfo"/>
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct ScrollInfo
        {
            /// <summary>
            /// Specifies the size, in bytes, of this structure. The caller must set this to <c>sizeof(</c><see cref="ScrollInfo"/><c>)</c>.
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// Specifies the scroll bar parameters to set or retrieve.
            /// <see cref="ScrollInfoMask"/>
            /// </summary>
            public uint fMask;
            /// <summary>
            /// Specifies the minimum scrolling position.
            /// </summary>
            public int nMin;
            /// <summary>
            /// Specifies the maximum scrolling position.
            /// </summary>
            public int nMax;
            /// <summary>
            /// Specifies the page size, in device units. A scroll bar uses this value to determine the appropriate size of the proportional scroll box.
            /// </summary>
            public uint nPage;
            /// <summary>
            /// Specifies the position of the scroll box.
            /// </summary>
            public int nPos;
            /// <summary>
            /// Specifies the immediate position of a scroll box that the user is dragging.
            /// An application can retrieve this value while processing the SB_THUMBTRACK request code.
            /// An application cannot set the immediate scroll position; the SetScrollInfo function ignores this member.
            /// </summary>
            public int nTrackPos;
        }

        /// <summary>
        /// Holds message data sent with <see cref="WM_NOTIFY"/>.
        /// </summary>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-nmhdr"/>
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct TagNMHDR
        {
            public IntPtr hwndFrom;
            public UIntPtr idFrom;
            public uint code;
        }

        /// <summary>
        /// Possible values of <see cref="ScrollInfo.fMask"/>.
        /// </summary>
        public enum ScrollInfoMask
        {
            /// <summary>
            /// The nMin and nMax members contain the minimum and maximum values for the scrolling range.
            /// </summary>
            SIF_RANGE = 0x1,
            /// <summary>
            /// The nPage member contains the page size for a proportional scroll bar.
            /// </summary>
            SIF_PAGE = 0x2,
            /// <summary>
            /// The nPos member contains the scroll box position, which is not updated while the user drags the scroll box.
            /// </summary>
            SIF_POS = 0x4,
            /// <summary>
            /// This value is used only when setting a scroll bar's parameters.
            /// If the scroll bar's new parameters make the scroll bar unnecessary,
            /// disable the scroll bar instead of removing it.
            /// </summary>
            SIF_DISABLENOSCROLL = 0x8,
            /// <summary>
            /// The nTrackPos member contains the current position of the scroll box while the user is dragging it.
            /// </summary>
            SIF_TRACKPOS = 0x10,
            /// <summary>
            /// Combination of SIF_PAGE, SIF_POS, SIF_RANGE, and SIF_TRACKPOS.
            /// </summary>
            SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
        }

        /// <summary>
        /// Possible values of the <c>nBar</c> parameter when calling <see cref="GetScrollInfo"/>.
        /// </summary>
        public enum ScrollInfoBar
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.<br/><br/>
        /// </summary>
        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, UIntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, UIntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, UIntPtr wParam, IntPtr lParam);

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, UIntPtr wParam, out IntPtr lParam);

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, MenuCmdId lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new UIntPtr(wParam), new IntPtr((uint)lParam));
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, IntPtr lParam)
        {
            return SendMessage(hWnd, Msg, new UIntPtr(wParam), lParam);
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, int lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new UIntPtr(wParam), new IntPtr(lParam));
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, out int lParam)
        {
            IntPtr retval = SendMessage(hWnd, (UInt32)Msg, new UIntPtr(wParam), out IntPtr outVal);
            lParam = outVal.ToInt32();
            return retval;
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, int lParam)
        {
            return SendMessage(hWnd, Msg, wParam, new IntPtr(lParam));
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam)
        {
            return SendMessage(hWnd, Msg, new UIntPtr(wParam), lParam);
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam)
        {
            return SendMessage(hWnd, Msg, new UIntPtr(wParam), lParam);
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, UIntPtr wParam, int lParam)
        {
            return SendMessage(hWnd, (uint)Msg, wParam, new IntPtr(lParam));
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, uint wParam, IntPtr lParam)
        {
            return SendMessage(hWnd, (uint)Msg, new UIntPtr(wParam), lParam);
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, uint wParam, string lParam)
        {
            return SendMessage(hWnd, (uint)Msg, new UIntPtr(wParam), lParam);
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, uint wParam, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lParam)
        {
            return SendMessage(hWnd, (uint)Msg, new UIntPtr(wParam), lParam);
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, uint wParam, int lParam)
        {
            return SendMessage(hWnd, (uint)Msg, new UIntPtr(wParam), new IntPtr(lParam));
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, UIntPtr wParam, IntPtr lParam)
        {
            return SendMessage(hWnd, (uint)Msg, wParam, lParam);
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref LangType lParam)
        {
            IntPtr retval = SendMessage(hWnd, Msg, new UIntPtr(wParam), out IntPtr outVal);
            lParam = (LangType)outVal;
            return retval;
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg)
        {
            return SendMessage(hWnd, Msg, UIntPtr.Zero, IntPtr.Zero);
        }

        /// <inheritdoc cref="SendMessage(IntPtr, uint, UIntPtr, string)"/>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam)
        {
            return SendMessage(hWnd, Msg, wParam, IntPtr.Zero);
        }

        public const int MAX_PATH = 260;

        [DllImport("kernel32")]
        public static extern uint GetACP();

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

        [DllImport("kernel32")]
        public static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);

        [DllImport("kernel32")]
        public static extern long WritePrivateProfileSection(string section, string val, string filePath);

        /// <summary>Indicates that the <c>uPosition</c> parameter gives the identifier of the menu item.</summary>
        public const uint MF_BYCOMMAND = 0;
        /// <summary>Indicates that the <c>uPosition</c> parameter gives the zero-based relative position of the menu item.</summary>
        public const uint MF_BYPOSITION = 0x00000400;
        /// <summary>Uses a bitmap as the menu item. The lpNewItem parameter contains a handle to the bitmap.</summary>
        public const uint MF_BITMAP = 0x00000004;
        /// <summary>Disables the menu item so that it cannot be selected, but this flag does not gray it.</summary>
        public const uint MF_DISABLED = 2;
        /// <summary>Enables the menu item so that it can be selected and restores it from its grayed state.</summary>
        public const uint MF_ENABLED = 0;
        /// <summary>Disables the menu item and grays it so that it cannot be selected.</summary>
        public const uint MF_GRAYED = 1;
        /// <summary>Functions the same as the <see cref="MF_MENUBREAK"/> flag for a menu bar. For a drop-down menu, submenu, or shortcut menu, the new column is separated from the old column by a vertical line.</summary>
        public const uint MF_MENUBARBREAK = 0x00000020;
        /// <summary>Places the item on a new line (for menu bars) or in a new column (for a drop-down menu, submenu, or shortcut menu) without separating columns.</summary>
        public const uint MF_MENUBREAK = 0x00000040;
        /// <summary>
        /// Specifies that the item is an owner-drawn item. Before the menu is displayed for the first time, the window that owns the menu receives a <see cref="WM_MEASUREITEM"/> message  to retrieve
        /// the width and height of the menu item. The <see cref="WM_DRAWITEM"/> message is then sent to the window procedure of the owner window whenever the appearance of the menu item must be updated.
        /// </summary>
        public const uint MF_OWNERDRAW = 0x00000100;
        /// <summary>
        /// Specifies that the menu item opens a drop-down menu or submenu. The <c>uIDNewItem</c> parameter specifies a handle to the drop-down menu or submenu. This flag is used to add a menu name to a
        /// menu bar or a menu item that opens a submenu to a drop-down menu, submenu, or shortcut menu.
        /// </summary>
        public const uint MF_POPUP = 0x00000010;
        /// <summary>
        /// Draws a horizontal dividing line. This flag is used only in a drop-down menu, submenu, or shortcut menu. The line cannot be grayed, disabled, or highlighted. The <c>lpNewItem</c> and
        /// <c>uIDNewItem</c> parameters are ignored.
        /// </summary>
        public const uint MF_SEPARATOR = 0x00000800;
        /// <summary>Specifies that the menu item is a text string; the <c>lpNewItem</c> parameter is a pointer to the string.</summary>
        public const uint MF_STRING = 0x00000000;
        /// <summary>
        /// Places a check mark next to the item. If your application provides check-mark bitmaps (see the
        /// <a href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-setmenuitembitmaps">SetMenuItemBitmaps</a> function),
        /// this flag displays a selected bitmap next to the menu item.
        /// </summary>
        public const uint MF_CHECKED = 8;
        /// <summary>
        /// Does not place a check mark next to the item (the default). If your application supplies check-mark bitmaps (see the
        /// <a href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-setmenuitembitmaps">SetMenuItemBitmaps</a> function),
        /// this flag displays a clear bitmap next to the menu item.
        /// </summary>
        public const uint MF_UNCHECKED = 0;

        public const int SIZE_RESTORED = 0;
        public const int SIZE_MINIMIZED = 1;
        public const int SIZE_MAXIMIZED = 2;
        public const int SIZE_MAXSHOW = 3;
        public const int SIZE_MAXHIDE = 4;

        [DllImport("user32", EntryPoint = "DefWindowProcW", CharSet = CharSet.Unicode)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Contains information about a menu item.
        /// </summary>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/windows/win32/api/winuser/ns-winuser-menuiteminfow"/>
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MenuItemInfo
        {
            /// <summary>
            /// The size of the structure, in bytes. The caller must set this member to <c>sizeof(MENUITEMINFO)</c>.
            /// </summary>
            public int cbSize;
            /// <summary>
            /// Indicates the members to be retrieved or set.
            /// </summary>
            public uint fMask;
            /// <summary>
            /// The menu item type.
            /// </summary>
            /// <remarks>
            /// Used only if <see cref="fMask"/> has a value of <see cref="MIIM_FTYPE"/>.
            /// </remarks>
            public uint fType;
            /// <summary>
            /// The menu item state.
            /// </summary>
            /// <remarks>
            /// Used only if <see cref="fMask"/> has a value of <see cref="MIIM_STATE"/>.
            /// </remarks>
            public uint fState;
            /// <summary>
            /// An application-defined value that identifies the menu item.
            /// </summary>
            /// <remarks>
            /// Used only if <see cref="fMask"/> has a value of <see cref="MIIM_ID"/>.
            /// </remarks>
            public int wID;
            /// <summary>
            /// A handle to the drop-down menu or submenu associated with the menu item. If the menu item is not an
            /// item that opens a drop-down menu or submenu, this member is <see cref="IntPtr.Zero"/>.
            /// </summary>
            /// <remarks>
            /// Used only if <see cref="fMask"/> has a value of <see cref="MIIM_SUBMENU"/>.
            /// </remarks>
            public IntPtr hSubMenu;
            /// <summary>
            /// A handle to the bitmap to display next to the item if it is selected. If this member is
            /// <see cref="IntPtr.Zero"/>, a default bitmap is used. If the <see cref="MFT_RADIOCHECK"/> type value is
            /// specified, the default bitmap is a bullet. Otherwise, it is a check mark.
            /// </summary>
            /// <remarks>
            /// Used only if <see cref="fMask"/> has a value of <see cref="MIIM_CHECKMARKS"/>.
            /// </remarks>
            public IntPtr hbmpChecked;
            /// <summary>
            /// A handle to the bitmap to display next to the item if it is not selected. If this member is
            /// <see cref="IntPtr.Zero"/>, no bitmap is used.
            /// </summary>
            /// <remarks>
            /// Used only if <see cref="fMask"/> has a value of <see cref="MIIM_CHECKMARKS"/>.
            /// </remarks>
            public IntPtr hbmpUnchecked;
            /// <summary>
            /// An application-defined value associated with the menu item.
            /// </summary>
            /// <remarks>
            /// Used only if <see cref="fMask"/> has a value of <see cref="MIIM_DATA"/>.
            /// </remarks>
            public UIntPtr dwItemData;
            /// <summary>
            /// The contents of the menu item. The meaning of this member depends on the value of <see cref="fType"/>
            /// and is used only if the <see cref="MIIM_TYPE"/> flag is set in the <see cref="fMask"/> member.
            /// </summary>
            /// <remarks>
            /// <see cref="dwTypeData"/> is used only if the <see cref="MIIM_STRING"/> flag is set in the
            /// <see cref="fMask"/> member.
            /// </remarks>
            public IntPtr dwTypeData;
            /// <summary>
            /// The length of the menu item text, in characters, when information is received about a menu item of the
            /// <see cref="MIIM_STRING"/> type.
            /// </summary>
            /// <remarks>
            /// The <see cref="cch"/> member is used when the <see cref="MIIM_STRING"/> flag is set in the
            /// <see cref="fMask"/> member.
            /// </remarks>
            public int cch;
            /// <summary>
            /// A handle to the bitmap to be displayed.
            /// </summary>
            /// <remarks>
            /// Used when the <see cref="MIIM_BITMAP"/> flag is set in the <see cref="fMask"/> member.
            /// </remarks>
            public IntPtr hbmpItem;
        }

        /// <summary>Retrieves or sets the <see cref="MenuItemInfo.hbmpItem"/> member.</summary>
        public const uint MIIM_BITMAP = 0x00000080;
        /// <summary>Retrieves or sets the <see cref="MenuItemInfo.hbmpChecked"/> and <see cref="MenuItemInfo.hbmpUnchecked"/> members.</summary>
        public const uint MIIM_CHECKMARKS = 0x00000008;
        /// <summary>Retrieves or sets the <see cref="MenuItemInfo.dwItemData"/> member.</summary>
        public const uint MIIM_DATA = 0x00000020;
        /// <summary>Retrieves or sets the <see cref="MenuItemInfo.fType"/> member.</summary>
        public const uint MIIM_FTYPE = 0x00000100;
        /// <summary>Retrieves or sets the <see cref="MenuItemInfo.wID"/> member.</summary>
        public const uint MIIM_ID = 0x00000002;
        /// <summary>Retrieves or sets the <see cref="MenuItemInfo.fState"/> member.</summary>
        public const uint MIIM_STATE = 0x00000001;
        /// <summary>Retrieves or sets the <see cref="MenuItemInfo.dwTypeData"/> member.</summary>
        public const uint MIIM_STRING = 0x00000040;
        /// <summary>Retrieves or sets the <see cref="MenuItemInfo.hSubMenu"/> member.</summary>
        public const uint MIIM_SUBMENU = 0x00000004;
        /// <summary>Retrieves or sets the <see cref="MenuItemInfo.fType"/> and <see cref="MenuItemInfo.dwTypeData"/> members.</summary>
        public const uint MIIM_TYPE = 0x00000010;
        /// <summary>Displays the menu item using a bitmap. The low-order word of the <see cref="MenuItemInfo.dwTypeData"/> member
        /// is the bitmap handle, and the <see cref="MenuItemInfo.cch"/> member is ignored.</summary>
        public const uint MFT_BITMAP = 0x00000004;
        /// <summary>Places the menu item on a new line (for a menu bar) or in a new column (for a drop-down menu,
        /// submenu, or shortcut menu). For a drop-down menu, submenu, or shortcut menu, a vertical line separates
        /// the new column from the old.</summary>
        public const uint MFT_MENUBARBREAK = 0x00000020;
        /// <summary>Places the menu item on a new line (for a menu bar) or in a new column (for a drop-down menu,
        /// submenu, or shortcut menu). For a drop-down menu, submenu, or shortcut menu, the columns are not separated
        /// by a vertical line.</summary>
        public const uint MFT_MENUBREAK = 0x00000040;
        /// <summary>Assigns responsibility for drawing the menu item to the window that owns the menu. The window
        /// receives a <see cref="WM_MEASUREITEM"/> message before the menu is displayed for the first time, and a <see cref="WM_DRAWITEM"/>
        /// message whenever the appearance of the menu item must be updated. If this value is specified, the
        /// dwTypeData member contains an application-defined value.</summary>
        public const uint MFT_OWNERDRAW = 0x00000100;
        /// <summary>Displays selected menu items using a radio-button mark instead of a check mark if the
        /// <see cref="MenuItemInfo.hbmpChecked"/> member is <see cref="IntPtr.Zero"/>.</summary>
        public const uint MFT_RADIOCHECK = 0x00000200;
        /// <summary>Right-justifies the menu item and any subsequent items. This value is valid only if the menu item
        /// is in a menu bar.</summary>
        public const uint MFT_RIGHTJUSTIFY = 0x00004000;
        /// <summary>Specifies that menus cascade right-to-left (the default is left-to-right). This is used to support
        /// right-to-left languages, such as Arabic and Hebrew.</summary>
        public const uint MFT_RIGHTORDER = 0x00002000;
        /// <summary>Specifies that the menu item is a separator. A menu item separator appears as a horizontal
        /// dividing line. The <see cref="MenuItemInfo.dwTypeData"/> and <see cref="MenuItemInfo.cch"/> members are
        /// ignored. This value is valid only in a drop-down menu, submenu, or shortcut menu.</summary>
        public const uint MFT_SEPARATOR = 0x00000800;
        /// <summary>Displays the menu item using a text string. The <see cref="MenuItemInfo.dwTypeData"/> member is
        /// the pointer to a null-terminated string, and the <see cref="MenuItemInfo.cch"/> member is the length of the
        /// string.</summary>
        public const uint MFT_STRING = 0x00000000;
        /// <summary>Checks the menu item. For more information about selected menu items, see the
        /// <see cref="MenuItemInfo.hbmpChecked"/> member.</summary>
        public const uint MFS_CHECKED = 0x00000008;
        /// <summary>Specifies that the menu item is the default. A menu can contain only one default menu item, which
        /// is displayed in bold.</summary>
        public const uint MFS_DEFAULT = 0x00001000;
        /// <summary>Disables the menu item and grays it so that it cannot be selected. This is equivalent to
        /// <see cref="MFS_GRAYED"/>.</summary>
        public const uint MFS_DISABLED = 0x00000003;
        /// <summary>Enables the menu item so that it can be selected. This is the default state.</summary>
        public const uint MFS_ENABLED = 0x00000000;
        /// <summary>Disables the menu item and grays it so that it cannot be selected. This is equivalent to
        /// <see cref="MFS_DISABLED"/>.</summary>
        public const uint MFS_GRAYED = 0x00000003;
        /// <summary>Highlights the menu item.</summary>
        public const uint MFS_HILITE = 0x00000080;
        /// <summary>Unchecks the menu item. For more information about clear menu items, see the
        /// <see cref="MenuItemInfo.hbmpChecked"/> member.</summary>
        public const uint MFS_UNCHECKED = 0x00000000;
        /// <summary>Removes the highlight from the menu item. This is the default state.</summary>
        public const uint MFS_UNHILITE = 0x00000000;

        [DllImport("user32")]
        public static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32", EntryPoint = "GetMenuItemInfoW", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMenuItemInfo(IntPtr hmenu, int item, [MarshalAs(UnmanagedType.Bool)] bool fByPosition, ref MenuItemInfo lpmii);

        [DllImport("user32", EntryPoint = "SetMenuItemInfoW", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMenuItemInfo(IntPtr hmenu, int item, [MarshalAs(UnmanagedType.Bool)] bool fByPosition, MenuItemInfo lpmii);

        [DllImport("user32", EntryPoint = "ModifyMenuW", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ModifyMenu(IntPtr hMenu, int uPosition, uint uFlags, UIntPtr uIDNewItem, IntPtr lpNewItem = default);

        [DllImport("user32", EntryPoint = "EnableMenuItem")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableMenuItem(IntPtr hMenu, int uIDEnableItem, uint uEnable);

        [DllImport("user32")]
        public static extern uint CheckMenuItem(IntPtr hmenu, int uIDCheckItem, uint uCheck);

        [DllImport("user32", EntryPoint = "MessageBoxW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern DlgResult MsgBoxDialog(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)] string lpText, [MarshalAs(UnmanagedType.LPTStr)] string lpCaption, uint uType);

        /// <summary>
        /// Possible values of the <c>uType</c> parameter of <see cref="MsgBoxDialog"/>.
        /// </summary>
        /// <remarks>
        /// To display an icon in the message box, specify one of the ICON* values.<br/>
        /// To indicate the default button, specify one of the DEFBUTTON* values.<br/>
        /// To indicate the modality of the dialog box, specify one of the following values beginning with <see cref="APPLMODAL"/>.<br/>
        /// See <see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-messagebox"/>
        /// </remarks>
        [Flags]
        public enum MsgBox : uint
        {
            /// <summary>The message box contains three push buttons: Abort, Retry, and Ignore.</summary>
            ABORTRETRYIGNORE = 0x00000002,
            /// <summary>The message box contains three push buttons: Cancel, Try Again, Continue. Use this message box type instead of <see cref="ABORTRETRYIGNORE"/>.</summary>
            CANCELTRYCONTINUE = 0x00000006,
            /// <summary>Adds a Help button to the message box. When the user clicks the Help button or presses F1, the system sends a <see cref="WM_HELP"/> message to the owner.</summary>
            HELP = 0x00004000,
            /// <summary>The message box contains one push button: OK. This is the default.</summary>
            OK = 0x00000000,
            /// <summary>The message box contains two push buttons: OK and Cancel.</summary>
            OKCANCEL = 0x00000001,
            /// <summary>The message box contains two push buttons: Retry and Cancel.</summary>
            RETRYCANCEL = 0x00000005,
            /// <summary>The message box contains two push buttons: Yes and No.</summary>
            YESNO = 0x00000004,
            /// <summary>The message box contains three push buttons: Yes, No, and Cancel.</summary>
            YESNOCANCEL = 0x00000003,
            /// <summary>An exclamation-point icon appears in the message box.</summary>
            ICONEXCLAMATION = 0x00000030,
            /// <summary>An exclamation-point icon appears in the message box.</summary>
            ICONWARNING = 0x00000030,
            /// <summary>An icon consisting of a lowercase letter <c>i</c> in a circle appears in the message box.</summary>
            ICONINFORMATION = 0x00000040,
            /// <summary>An icon consisting of a lowercase letter <c>i</c> in a circle appears in the message box.</summary>
            ICONASTERISK = 0x00000040,
            /// <summary>A question-mark icon appears in the message box.</summary>
            /// <remarks>
            /// The question-mark message icon is no longer recommended because it does not clearly represent a specific
            /// type of message and because the phrasing of a message as a question could apply to any message type.
            /// In addition, users can confuse the message symbol question mark with Help information. Therefore, do not use
            /// this question mark message symbol in your message boxes. The system continues to support its inclusion only
            /// for backward compatibility.
            /// </remarks>
            ICONQUESTION = 0x00000020,
            /// <summary>A stop-sign icon appears in the message box.</summary>
            ICONSTOP = 0x00000010,
            /// <summary>A stop-sign icon appears in the message box.</summary>
            ICONERROR = 0x00000010,
            /// <summary>A stop-sign icon appears in the message box.</summary>
            ICONHAND = 0x00000010,
            /// <summary>
            /// The first button is the default button.
            /// DEFBUTTON1 is the default unless DEFBUTTON2, DEFBUTTON3, or DEFBUTTON4 is specified.
            /// </summary>
            DEFBUTTON1 = 0x00000000,
            /// <summary>The second button is the default button.</summary>
            DEFBUTTON2 = 0x00000100,
            /// <summary>The third button is the default button.</summary>
            DEFBUTTON3 = 0x00000200,
            /// <summary>The fourth button is the default button.</summary>
            DEFBUTTON4 = 0x00000300,
            /// <summary>
            /// The user must respond to the message box before continuing work in the window identified by the hWnd parameter.
            /// However, the user can move to the windows of other threads and work in those windows.
            /// <see cref="APPLMODAL"/> is the default if neither <see cref="SYSTEMMODAL"/> nor <see cref="TASKMODAL"/> is specified.
            /// </summary>
            /// <remarks>
            /// Depending on the hierarchy of windows in the application, the user may be able to move to other windows within the thread.
            /// All child windows of the parent of the message box are automatically disabled, but pop-up windows are not.
            /// </remarks>
            APPLMODAL = 0x00000000,
            /// <summary>
            /// Same as <see cref="APPLMODAL"/> except that the message box has the <see cref="WS_EX_TOPMOST"/> style. Use system-modal message boxes to notify the user of serious, potentially damaging errors that require immediate attention (for example, running out of memory). This flag has no effect on the user's ability to interact with windows other than those associated with hWnd.
            /// </summary>
            SYSTEMMODAL = 0x00001000,
            /// <summary>
            /// Same as <see cref="APPLMODAL"/> except that all the top-level windows belonging to the current thread are disabled if the hWnd parameter is NULL. Use this flag when the calling application or library does not have a window handle available but still needs to prevent input to other windows in the calling thread without suspending other threads.
            /// </summary>
            TASKMODAL = 0x00002000,
            /// <summary>
            /// Same as desktop of the interactive window station. For more information, see Window Stations.
            /// </summary>
            /// <remarks>
            /// If the current input desktop is not the default desktop, <see cref="MsgBoxDialog"/> does not return until the user switches to the default desktop.
            /// </remarks>
            DEFAULT_DESKTOP_ONLY = 0x00020000,
            /// <summary>The text is right-justified.</summary>
            RIGHT = 0x00080000,
            /// <summary>Displays message and caption text using right-to-left reading order on Hebrew and Arabic systems.</summary>
            RTLREADING = 0x00100000,
            /// <summary>The message box becomes the foreground window. Internally, the system calls the SetForegroundWindow function for the message box.</summary>
            SETFOREGROUND = 0x00010000,
            /// <summary>The message box is created with the <see cref="WS_EX_TOPMOST"/> window style.</summary>
            TOPMOST = 0x00040000,
            /// <summary>The caller is a service notifying the user of an event. The function displays a message box on the current active desktop, even if there is no user logged on to the computer.</summary>
            SERVICE_NOTIFICATION = 0x00200000
        }

        /// <summary>
        /// Possible return values of <see cref="MsgBoxDialog"/>.
        /// </summary>
        public enum DlgResult
        {
            /// <summary>The Abort button was selected.</summary>
            ABORT = 3,
            /// <summary>The Cancel button was selected.</summary>
            CANCEL = 2,
            /// <summary>The Continue button was selected.</summary>
            CONTINUE = 11,
            /// <summary>The Ignore button was selected.</summary>
            IGNORE = 5,
            /// <summary>The No button was selected.</summary>
            NO = 7,
            /// <summary>The OK button was selected.</summary>
            OK = 1,
            /// <summary>The Retry button was selected.</summary>
            RETRY = 4,
            /// <summary>The Try Again button was selected.</summary>
            TRYAGAIN = 10,
            /// <summary>The Yes button was selected.</summary>
            YES = 6
        }

        public const uint WM_NULL = 0;
        public const uint WM_CREATE = 1;
        public const uint WM_DESTROY = 2;
        public const uint WM_MOVE = 3;
        public const uint WM_SIZE = 5;
        public const uint WM_ACTIVATE = 6;
        public const uint WM_SETFOCUS = 7;
        public const uint WM_KILLFOCUS = 8;
        public const uint WM_ENABLE = 10;
        public const uint WM_SETREDRAW = 11;
        public const uint WM_SETTEXT = 12;
        public const uint WM_GETTEXT = 13;
        public const uint WM_GETTEXTLENGTH = 14;
        public const uint WM_PAINT = 15;
        public const uint WM_CLOSE = 16;
        public const uint WM_QUERYENDSESSION = 17;
        public const uint WM_QUIT = 18;
        public const uint WM_QUERYOPEN = 19;
        public const uint WM_ERASEBKGND = 20;
        public const uint WM_SYSCOLORCHANGE = 21;
        public const uint WM_ENDSESSION = 22;
        public const uint WM_SHOWWINDOW = 24;
        public const uint WM_CTLCOLOR = 25;
        public const uint WM_WININICHANGE = 26;
        public const uint WM_DEVMODECHANGE = 27;
        public const uint WM_ACTIVATEAPP = 28;
        /// <summary>
        /// Sent to the parent window of an owner-drawn button, combo box, list box, or menu when a visual aspect of the button, combo box, list box, or menu has changed.
        /// See <see href="https://learn.microsoft.com/windows/win32/controls/wm-drawitem"/>
        /// </summary>
        public const uint WM_DRAWITEM = 43;
        /// <summary>
        /// Sent to the owner window of a combo box, list box, list-view control, or menu item when the control or menu is created.
        /// See <see href="https://learn.microsoft.com/windows/win32/controls/wm-measureitem"/>
        /// </summary>
        public const uint WM_MEASUREITEM = 44;
        public const int WM_NOTIFY = 0x004e;
        public const int WM_HELP = 0x0053;
        public const int GWL_EXSTYLE = -20;
        public const int GWLP_HINSTANCE = -6;
        public const int GWLP_HWNDPARENT = -8;
        public const int GWLP_ID = -12;
        public const int GWL_STYLE = -16;
        public const int GWLP_USERDATA = -21;
        public const int GWLP_WNDPROC = -4;
        public const long WS_EX_ACCEPTFILES = 0x00000010L;
        public const long WS_EX_APPWINDOW = 0x00040000L;
        public const long WS_EX_CLIENTEDGE = 0x00000200L;
        public const long WS_EX_COMPOSITED = 0x02000000L;
        public const long WS_EX_CONTEXTHELP = 0x00000400L;
        public const long WS_EX_CONTROLPARENT = 0x00010000L;
        public const long WS_EX_DLGMODALFRAME = 0x00000001L;
        public const long WS_EX_LAYERED = 0x00080000L;
        public const long WS_EX_LAYOUTRTL = 0x00400000L;
        public const long WS_EX_LEFT = 0x00000000L;
        public const long WS_EX_LEFTSCROLLBAR = 0x00004000L;
        public const long WS_EX_LTRREADING = 0x00000000L;
        public const long WS_EX_MDICHILD = 0x00000040L;
        public const long WS_EX_NOACTIVATE = 0x08000000L;
        public const long WS_EX_NOINHERITLAYOUT = 0x00100000L;
        public const long WS_EX_NOPARENTNOTIFY = 0x00000004L;
        public const long WS_EX_NOREDIRECTIONBITMAP = 0x00200000L;
        public const long WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const long WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const long WS_EX_RIGHT = 0x00001000L;
        public const long WS_EX_RIGHTSCROLLBAR = 0x00000000L;
        public const long WS_EX_RTLREADING = 0x00002000L;
        public const long WS_EX_STATICEDGE = 0x00020000L;
        public const long WS_EX_TOOLWINDOW = 0x00000080L;
        public const long WS_EX_TOPMOST = 0x00000008L;
        public const long WS_EX_TRANSPARENT = 0x00000020L;
        public const long WS_EX_WINDOWEDGE = 0x00000100L;

        public delegate IntPtr WindowLongGetter(IntPtr hWnd, int nIndex);
        public delegate IntPtr WindowLongSetter(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("kernel32")]
        public static extern void OutputDebugString(string lpOutputString);

        /// <summary>
        /// See <see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-getscrollinfo"/>
        /// </summary>
        /// <param name="hwnd">Handle to a scroll bar control or a window with a standard scroll bar, depending on the value of the <paramref name="nBar"/> parameter.</param>
        /// <param name="nBar">See <see cref="ScrollInfoBar"/></param>
        /// <param name="scrollInfo">See <see cref="ScrollInfo"/></param>
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetScrollInfo(IntPtr hwnd, int nBar, ref ScrollInfo scrollInfo);
    }
}
