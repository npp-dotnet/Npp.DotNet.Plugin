﻿/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Runtime.InteropServices;

namespace Npp.DotNet.Plugin.Winforms
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
        /// Display an icon on the docking tab (i.e., when this dialog loses focus in a dock group)
        /// </summary>
        DWS_ICONTAB = 0x00000001,
        /// <summary>
        /// Display an icon in the title bar (currently not supported)
        /// </summary>
        DWS_ICONBAR = 0x00000002,
        /// <summary>
        /// Display a string of additional information in the title bar
        /// </summary>
        DWS_ADDINFO = 0x00000004,
        /// <summary>
        /// Use plugin's own dark mode (i.e., prevent automatic subclassing; see <see cref="NppMsg.NPPM_DARKMODESUBCLASSANDTHEME"/>)
        /// </summary>
        DWS_USEOWNDARKMODE = 0x00000008,
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

    /// <summary>
    /// Messages that can be sent to docking dialogs in the <see cref="Win32.TagNMHDR.Code"/> field of a <see cref="Win32.TagNMHDR"/>.
    /// </summary>
    /// <remarks>
    /// <example>
    /// Example
    /// <code>
    ///  protected override void WndProc(ref Message wmNotifyHeader)
    ///  {
    ///    switch (wmNotifyHeader.Msg)
    ///    {
    ///      case WM_NOTIFY:
    ///        TagNMHDR nmdr = Marshal.PtrToStructure&lt;TagNMHDR&gt;(wmNotifyHeader.LParam);
    ///        if (nmdr.HwndFrom == PluginData.NppData.NppHandle)
    ///        {
    ///          switch ((DockMgrMsg)nmdr.Code)
    ///          {
    ///            case DockMgrMsg.DMN_DOCK:
    ///              // ...
    ///              break;
    ///            case DockMgrMsg.DMN_FLOAT:
    ///              // ...
    ///              break;
    ///            case DockMgrMsg.DMN_CLOSE:
    ///              // ...
    ///              break;
    ///          }
    ///        }
    ///        break;
    ///    }
    ///    base.WndProc(ref wmNotifyHeader);
    ///  }
    /// </code>
    /// </example>
    /// </remarks>
    [Flags]
    public enum DockMgrMsg : uint
    {
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
        DMN_FIRST = 1050,
        DMN_CLOSE = (DMN_FIRST + 1),
        DMN_DOCK = (DMN_FIRST + 2),
        DMN_FLOAT = (DMN_FIRST + 3),
        DMN_SWITCHIN  = (DMN_FIRST + 4),
        DMN_SWITCHOFF = (DMN_FIRST + 5),
        DMN_FLOATDROPPED = (DMN_FIRST + 6)
    }

    /// <summary>
    /// Resource IDs for pre-defined dialog components.
    /// </summary>
    public enum DockingResource : ushort
    {
        IDB_CLOSE_DOWN = 137,
        IDB_CLOSE_UP = 138,
        IDD_CONTAINER_DLG = 139,
        IDC_TAB_CONT = 1027,
        IDC_CLIENT_TAB = 1028,
        IDC_BTN_CAPTION = 1050
    }


    /// <summary>
    /// See <see cref="NppMsg.NPPM_DMMREGASDCKDLG"/>.
    /// </summary>
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
        /// User-provided index of the plugin command associated with this toolbar item &#2014; <em>not to be confused with <see cref="FuncItem.CmdID"/></em>
        /// </summary>
        public int DlgID;
        /// <summary>
        /// Mask params. See <see cref="NppTbMsg"/>
        /// </summary>
        public NppTbMsg UMask;
        /// <summary>
        /// Icon for tabs, provided the <see cref="NppTbMsg.DWS_ICONTAB"/> flag is set
        /// </summary>
        public IntPtr HIconTab;
        /// <summary>
        /// Additional information (shown in window's title bar), provided the <see cref="NppTbMsg.DWS_ADDINFO"/> flag is set
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
