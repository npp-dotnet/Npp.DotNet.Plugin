/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Runtime.InteropServices;
using static Npp.DotNet.Plugin.Win32;

namespace Npp.DotNet.Plugin.Winforms
{
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/windows/win32/api/winuser"/>
    /// </remarks>
    public static class WinUser
    {
        /// <summary>
        /// Loads an icon, cursor, animated cursor, or bitmap.
        /// </summary>
        /// <param name="hInst">
        /// A handle to the module of either a DLL or executable (.exe) that contains the image to be loaded.
        /// To load a predefined image or a standalone resource (icon, cursor, or bitmap file), set this parameter to <see cref="NULL"/>.
        /// </param>
        /// <param name="name">
        /// The image to be loaded.<br/><br/>
        /// If <paramref name="hInst"/> is not <see cref="NULL"/> and <paramref name="loadFlags"/> omits <see cref="LoadImageFlag.LR_LOADFROMFILE"/>,
        /// the name specifies the image resource in the <paramref name="hInst"/> module.<br/><br/>
        /// If <paramref name="hInst"/> is <see cref="NULL"/> and <paramref name="loadFlags"/> omits <see cref="LoadImageFlag.LR_LOADFROMFILE"/> and
        /// includes <see cref="LoadImageFlag.LR_SHARED"/>, the name specifies the predefined image to load.
        /// </param>
        /// <param name="type">The type of image to be loaded.</param>
        /// <param name="cx">Width, in pixels, of the icon or cursor.</param>
        /// <param name="cy">Height, in pixels, of the icon or cursor.</param>
        /// <param name="loadFlags">One or more values of <see cref="LoadImageFlag"/>.</param>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-loadimagew"/>
        /// </remarks>
        [DllImport("user32", EntryPoint = "LoadImageW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadImage(IntPtr hInst, [MarshalAs(UnmanagedType.LPTStr)] string name, [MarshalAs(UnmanagedType.U4)] LoadImageType type, int cx, int cy, [MarshalAs(UnmanagedType.U4)] LoadImageFlag loadFlags);

        /// <summary>
        /// Convenience method for loading a predefined Windows icon.
        /// </summary>
        /// <param name="iconID">The <see cref="WindowsIcon"/>to load.</param>
        public static IntPtr GetStandardIcon(WindowsIcon iconID)
        {
            return LoadWindowsIcon(NULL, iconID, LoadImageType.IMAGE_ICON, 0, 0, LoadImageFlag.LR_SHARED | LoadImageFlag.LR_DEFAULTSIZE);
        }

        /// <summary>
        /// Convenience method for loading a predefined Windows cursor.
        /// </summary>
        /// <param name="cursorID">The <see cref="WindowsCursor"/> to load.</param>
        public static IntPtr GetStandardCursor(WindowsCursor cursorID)
        {
            return LoadWindowsCursor(NULL, cursorID, LoadImageType.IMAGE_CURSOR, 0, 0, LoadImageFlag.LR_SHARED | LoadImageFlag.LR_DEFAULTSIZE);
        }

        /// <summary>
        /// Retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window whose DC is to be retrieved. If this value is <see cref="NULL"/>., GetDC retrieves the DC for the entire screen.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the DC for the specified window's client area.
        /// If the function fails, the return value is <see cref="NULL"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// Releases a device context (DC), freeing it for use by other applications.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
        /// <param name="hdc">A handle to the DC to be released.</param>
        /// <returns>
        /// If the DC was released, the return value is 1.
        /// If the DC was not released, the return value is zero.
        /// </returns>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-releasedc"/>
        /// </remarks>
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);

        [DllImport("user32", EntryPoint = "LoadImageW", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr LoadWindowsIcon(IntPtr hInst, [MarshalAs(UnmanagedType.LPTStr)] WindowsIcon iconID, [MarshalAs(UnmanagedType.U4)] LoadImageType type, int cx, int cy, [MarshalAs(UnmanagedType.U4)] LoadImageFlag loadFlags);

        [DllImport("user32", EntryPoint = "LoadImageW", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr LoadWindowsCursor(IntPtr hInst, [MarshalAs(UnmanagedType.LPTStr)] WindowsCursor cursorID, [MarshalAs(UnmanagedType.U4)] LoadImageType type, int cx, int cy, [MarshalAs(UnmanagedType.U4)] LoadImageFlag loadFlags);

        /// <summary>
        /// Possible values of the <c>loadFlags</c> parameter when calling <see cref="LoadImage"/>.
        /// </summary>
        [Flags]
        public enum LoadImageFlag : uint
        {
            /// <summary>
            /// When the <c>type</c> parameter specifies <see cref="LoadImageType.IMAGE_BITMAP"/>, causes the function to return a DIB section bitmap
            /// rather than a compatible bitmap. This flag is useful for loading a bitmap without mapping it to the colors of the display device.
            /// </summary>
            LR_CREATEDIBSECTION = 0x00002000,
            /// <summary>
            /// The default flag; it does nothing. All it means is "not <see cref="LR_MONOCHROME"/>".
            /// </summary>
            LR_DEFAULTCOLOR = 0x00000000,
            /// <summary>
            /// Uses the width or height specified by the system metric values for cursors or icons, if the <c>cx</c> or <c>cy</c> values are set to
            /// zero. If this flag is not specified and <c>cx</c> and <c>cy</c> are set to zero, the function uses the actual resource size. If the
            /// resource contains multiple images, the function uses the size of the first image.
            /// </summary>
            LR_DEFAULTSIZE = 0x00000040,
            /// <summary>
            /// Loads the standalone image from the file specified by name (icon, cursor, or bitmap file).
            /// </summary>
            LR_LOADFROMFILE = 0x00000010,
            /// <summary>
            ///  Searches the color table for the image and replaces the following shades of gray with the corresponding 3-D color.
            /// <example>
            ///   <code>
            ///     Dk Gray, RGB(128,128,128) with COLOR_3DSHADOW
            ///     Gray, RGB(192,192,192) with COLOR_3DFACE
            ///     Lt Gray, RGB(223,223,223) with COLOR_3DLIGHT
            ///   </code>
            /// </example>
            /// </summary>
            LR_LOADMAP3DCOLORS = 0x00001000,
            /// <summary>
            /// Retrieves the color value of the first pixel in the image and replaces the corresponding entry in the color table with the default
            /// window color (<see cref="COLOR_WINDOW"/>). All pixels in the image that use that entry become the default window color. This value
            /// applies only to images that have corresponding color tables.<br/><br/>
            /// If <c>loadFlags</c> includes both the <see cref="LR_LOADTRANSPARENT"/> and <see cref="LR_LOADMAP3DCOLORS"/> values, <see cref="LR_LOADTRANSPARENT"/>
            /// takes precedence. However, the color table entry is replaced with <see cref="COLOR_3DFACE"/> rather than <see cref="COLOR_WINDOW"/>.
            /// </summary>
            /// <remarks>
            /// Do not use this option if you are loading a bitmap with a color depth greater than 8bpp.
            /// </remarks>
            LR_LOADTRANSPARENT = 0x00000020,
            /// <summary>
            /// Loads the image in black and white.
            /// </summary>
            LR_MONOCHROME = 0x00000001,
            /// <summary>
            /// Shares the image handle if the image is loaded multiple times. If <see cref="LR_SHARED"/> is not set, a second call to
            /// <see cref="LoadImage"/> for the same resource will load the image again and return a different handle.<br/><br/>
            /// When you use this flag, the system will destroy the resource when it is no longer needed.<br/><br/>
            /// When loading a system icon or cursor, you must use <see cref="LR_SHARED"/> or the function will fail to load the resource.<br/><br/>
            /// This function finds the first image in the cache with the requested resource name, regardless of the size requested.
            /// </summary>
            /// <remarks>
            /// Do not use this option for images that have non-standard sizes, that may change after loading, or that are loaded from a file.
            /// </remarks>
            LR_SHARED = 0x00008000,
            /// <summary>
            /// Uses true VGA colors.
            /// </summary>
            LR_VGACOLOR = 0x00000080
        }

        /// <summary>
        /// Possible values of the <c>type</c> parameter when calling <see cref="LoadImage"/>.
        /// </summary>
        public enum LoadImageType : uint
        {
            IMAGE_BITMAP,
            IMAGE_ICON,
            IMAGE_CURSOR,
            IMAGE_ENHMETAFILE
        }

        /// <summary>
        /// Standard icon IDs
        /// </summary>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/windows/win32/menurc/about-icons#icon-types"/>
        /// </remarks>
        public enum WindowsIcon : ushort
        {
            IDI_APPLICATION = 32512,
            IDI_HAND = 32513,
            IDI_QUESTION = 32514,
            IDI_EXCLAMATION = 32515,
            IDI_ASTERISK = 32516,
            IDI_WINLOGO = 32517,
            IDI_SHIELD = 32518,
            IDI_WARNING = IDI_EXCLAMATION,
            IDI_ERROR = IDI_HAND,
            IDI_INFORMATION = IDI_ASTERISK
        }

        /// <summary>
        /// Standard cursor IDs
        /// </summary>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/about-cursors"/>
        /// </remarks>
        public enum WindowsCursor : ushort
        {
            IDC_ARROW = 32512,
            IDC_IBEAM = 32513,
            IDC_WAIT = 32514,
            IDC_CROSS = 32515,
            IDC_UPARROW = 32516,
            [Obsolete("Use IDC_SIZEALL")]
            IDC_SIZE = 32640,  /* OBSOLETE: use IDC_SIZEALL */
            [Obsolete("Use IDC_ARROW")]
            IDC_ICON = 32641,  /* OBSOLETE: use IDC_ARROW */
            IDC_SIZENWSE = 32642,
            IDC_SIZENESW = 32643,
            IDC_SIZEWE = 32644,
            IDC_SIZENS = 32645,
            IDC_SIZEALL = 32646,
            IDC_NO = 32648, /*not in win3.1 */
            IDC_HAND = 32649,
            IDC_APPSTARTING = 32650, /*not in win3.1 */
            IDC_HELP = 32651,
            IDC_PIN = 32671,
            IDC_PERSON = 32672
        }

        #region Color Types
        public const int CTLCOLOR_MSGBOX = 0;
        public const int CTLCOLOR_EDIT = 1;
        public const int CTLCOLOR_LISTBOX = 2;
        public const int CTLCOLOR_BTN = 3;
        public const int CTLCOLOR_DLG = 4;
        public const int CTLCOLOR_SCROLLBAR = 5;
        public const int CTLCOLOR_STATIC = 6;
        public const int CTLCOLOR_MAX = 7;
        public const int COLOR_SCROLLBAR = 0;
        public const int COLOR_BACKGROUND = 1;
        public const int COLOR_ACTIVECAPTION = 2;
        public const int COLOR_INACTIVECAPTION = 3;
        public const int COLOR_MENU = 4;
        public const int COLOR_WINDOW = 5;
        public const int COLOR_WINDOWFRAME = 6;
        public const int COLOR_MENUTEXT = 7;
        public const int COLOR_WINDOWTEXT = 8;
        public const int COLOR_CAPTIONTEXT = 9;
        public const int COLOR_ACTIVEBORDER = 10;
        public const int COLOR_INACTIVEBORDER = 11;
        public const int COLOR_APPWORKSPACE = 12;
        public const int COLOR_HIGHLIGHT = 13;
        public const int COLOR_HIGHLIGHTTEXT = 14;
        public const int COLOR_BTNFACE = 15;
        public const int COLOR_BTNSHADOW = 16;
        public const int COLOR_GRAYTEXT = 17;
        public const int COLOR_BTNTEXT = 18;
        public const int COLOR_INACTIVECAPTIONTEXT = 19;
        public const int COLOR_BTNHIGHLIGHT = 20;
        public const int COLOR_3DDKSHADOW = 21;
        public const int COLOR_3DLIGHT = 22;
        public const int COLOR_INFOTEXT = 23;
        public const int COLOR_INFOBK = 24;
        public const int COLOR_HOTLIGHT = 26;
        public const int COLOR_GRADIENTACTIVECAPTION = 27;
        public const int COLOR_GRADIENTINACTIVECAPTION = 28;
        public const int COLOR_MENUHILIGHT = 29;
        public const int COLOR_MENUBAR = 30;
        public const int COLOR_DESKTOP = COLOR_BACKGROUND;
        public const int COLOR_3DFACE = COLOR_BTNFACE;
        public const int COLOR_3DSHADOW = COLOR_BTNSHADOW;
        public const int COLOR_3DHIGHLIGHT = COLOR_BTNHIGHLIGHT;
        public const int COLOR_3DHILIGHT = COLOR_BTNHIGHLIGHT;
        public const int COLOR_BTNHILIGHT = COLOR_BTNHIGHLIGHT;
        #endregion
    }
}
