/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Runtime.InteropServices;

namespace Npp.DotNet.Plugin
{
    /// <summary>
    /// Defines for the Docking Manager.<br/>
    /// <c>CONT_*</c> :  styles for containers<br/>
    /// <c>DWS_*</c>  :  mask params for plugins of internal dialogs<br/>
    /// <c>DWS_DF_*</c> : default docking values for first call of plugin
    /// </summary>
    [Flags]
    public enum NppTbMsg : uint
    {
        /// <summary>
        /// Icon for tabs are available
        /// </summary>
        DWS_ICONTAB = 0x00000001,
        /// <summary>
        /// Icon for icon bar are available (currently not supported)
        /// </summary>
        DWS_ICONBAR = 0x00000002,
        /// <summary>
        /// Additional information are in use
        /// </summary>
        DWS_ADDINFO = 0x00000004,
        /// <summary>
        /// Combines all properties of <see cref="DWS_ICONTAB"/>, <see cref="DWS_ICONBAR"/> and <see cref="DWS_ADDINFO"/>
        /// </summary>
        DWS_PARAMSALL = (DWS_ICONTAB | DWS_ICONBAR | DWS_ADDINFO),
        /// <summary>
        /// Default docking on left
        /// </summary>
        DWS_DF_CONT_LEFT = (Constants.CONT_LEFT << 28),
        /// <summary>
        /// Default docking on right
        /// </summary>
        DWS_DF_CONT_RIGHT = (Constants.CONT_RIGHT << 28),
        /// <summary>
        /// Default docking on top
        /// </summary>
        DWS_DF_CONT_TOP = (Constants.CONT_TOP << 28),
        /// <summary>
        /// Default docking on bottom
        /// </summary>
        DWS_DF_CONT_BOTTOM = (Constants.CONT_BOTTOM << 28),
        /// <summary>
        /// Default state is floating
        /// </summary>
        DWS_DF_FLOATING = 0x80000000
    }

    [Flags]
    public enum DockMgrMsg : uint
    {
        IDB_CLOSE_DOWN = 137,
        IDB_CLOSE_UP = 138,
        IDD_CONTAINER_DLG = 139,
        IDC_TAB_CONT = 1027,
        IDC_CLIENT_TAB = 1028,
        IDC_BTN_CAPTION = 1050,
        DMM_MSG = 0x5000,
        DMM_CLOSE = (DMM_MSG + 1),
        DMM_DOCK = (DMM_MSG + 2),
        DMM_FLOAT = (DMM_MSG + 3),
        DMM_DOCKALL = (DMM_MSG + 4),
        DMM_FLOATALL = (DMM_MSG + 5),
        DMM_MOVE = (DMM_MSG + 6),
        DMM_UPDATEDISPINFO = (DMM_MSG + 7),
        DMM_GETIMAGELIST = (DMM_MSG + 8),
        DMM_GETICONPOS = (DMM_MSG + 9),
        DMM_DROPDATA = (DMM_MSG + 10),
        DMM_MOVE_SPLITTER = (DMM_MSG + 11),
        DMM_CANCEL_MOVE = (DMM_MSG + 12),
        DMM_LBUTTONUP = (DMM_MSG + 13),
        /// <summary>
        /// <example>
        /// Usage: <c>nmhdr.Code = DWORD(DMN_XXX, int newContainer)</c><br/>
        /// e.g.,
        /// <code>
        ///     nmhdr.Code = DWORD(DMN_CLOSE, 0));
        ///     nmhdr.hwndFrom = hwndNpp;
        ///     nmhdr.IdFrom = ctrlIdNpp;
        /// </code>
        /// </example>
        /// </summary>
        DMN_FIRST = 1050,
        DMN_CLOSE = (DMN_FIRST + 1),
        DMN_DOCK = (DMN_FIRST + 2),
        DMN_FLOAT = (DMN_FIRST + 3)
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NppTbData
    {
        /// <summary>
        /// Client window handle
        /// </summary>
        public IntPtr HClient;
        /// <summary>
        /// Name of plugin (shown in window's title bar)
        /// </summary>
        public string PszName;
        /// <summary>
        /// Command id of the plugin function that will launch the window
        /// </summary>
        public int DlgID;
        /// <summary>
        /// Mask params. See <see cref="NppTbMsg"/>
        /// </summary>
        public NppTbMsg UMask;
        /// <summary>
        /// Icon for tabs
        /// </summary>
        public uint HIconTab;
        /// <summary>
        /// Additional information (shown in window's title bar)
        /// </summary>
        public string PszAddInfo;
        /// <summary>
        /// Floating position
        /// </summary>
        /// <remarks>
        /// Internal data, DO NOT USE
        /// </remarks>
        public RECT RcFloat;
        /// <summary>
        /// Stores the previous container (toggling between float and dock)
        /// </summary>
        /// <remarks>
        /// Internal data, DO NOT USE
        /// </remarks>
        public int IPrevCont;
        /// <summary>
        /// Plugin file name, including '.dll'
        /// </summary>
        public string PszModuleName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public RECT(int left, int top, int right, int bottom)
        {
            Left = left; Top = top; Right = right; Bottom = bottom;
        }
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
