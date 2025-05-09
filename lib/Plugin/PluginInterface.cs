﻿/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>, et al.
 *                         2024 Robert Di Pardo <dipardo.r@gmail.com>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Npp.DotNet.Plugin
{
    /// <summary>
    /// A container of static, globally visible data.
    /// </summary>
    public static class PluginData
    {
        public static NppData NppData { get; set; }
        public static PluginFuncArray FuncItems { get => _funcArray = _funcArray ?? new PluginFuncArray(); }

        /// <summary>
        /// Gets an instance of <see cref="IScintillaGateway"/> created from the active Scintilla handle.
        /// </summary>
        public static IScintillaGateway Editor { get => new ScintillaGateway(Utils.GetCurrentScintilla()); }

        /// <summary>
        /// Gets an instance of <see cref="INotepadPPGateway"/>.
        /// </summary>
        public static INotepadPPGateway Notepad { get; } = new NotepadPPGateway();

        /// <summary>
        /// Provides global access to an allocated pointer to the plugin's name string.
        /// Plugins can deallocate it by setting this property to <see cref="IntPtr.Zero"/>.
        /// </summary>
        public static IntPtr PluginNamePtr
        {
            get
            {
                if (PszPluginName == IntPtr.Zero)
                    PszPluginName = Marshal.StringToHGlobalUni(PluginData.DefaultPluginName);
                return PszPluginName;
            }
            set
            {
                if (PszPluginName != IntPtr.Zero)
                    Marshal.FreeHGlobal(PszPluginName);
                PszPluginName = value;
            }
        }

        private static PluginFuncArray _funcArray;
        internal static IntPtr PszPluginName;
        internal static readonly string DefaultPluginName = "Npp.DotNet.Plugin\0";
    }

    public enum NativeBool : byte
    {
        False, True
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NppData
    {
        public IntPtr NppHandle, ScintillaMainHandle, ScintillaSecondHandle;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ShortcutKey
    {
        public ShortcutKey(NativeBool ctrl, NativeBool alt, NativeBool shift, byte ch)
        {
            IsCtrl = ctrl;
            IsAlt = alt;
            IsShift = shift;
            Key = ch;
        }
        public ShortcutKey(bool ctrl, bool alt, bool shift, Keys ch)
        {
            if (!Enum.TryParse($"{ctrl}", true, out IsCtrl)) IsCtrl = NativeBool.False;
            if (!Enum.TryParse($"{alt}", true, out IsAlt)) IsAlt = NativeBool.False;
            if (!Enum.TryParse($"{shift}", true, out IsShift)) IsShift = NativeBool.False;
            Key = (byte)ch;
        }
        [FieldOffset(0)] public NativeBool IsCtrl;
        [FieldOffset(1)] public NativeBool IsAlt;
        [FieldOffset(2)] public NativeBool IsShift;
        [FieldOffset(3)] public byte Key;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FuncItem
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.MENU_TITLE_LENGTH)]
        public string ItemName;
        public PluginFunc PFunc;
        public int CmdID;
        [MarshalAs(UnmanagedType.U1)]
        public NativeBool Init2Check;
        public ShortcutKey PShKey;
    }

    /// <remarks>
    /// See <see cref="NppMsg.NPPM_ADDTOOLBARICON_FORDARKMODE"/>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct ToolbarIconDarkMode
    {
        /// <summary>
        /// Standard 16x16 bitmap handle
        /// </summary>
        public IntPtr HToolbarBmp;
        /// <summary>
        /// Fluent UI icon handle for light mode
        /// </summary>
        public IntPtr HToolbarIcon;
        /// <summary>
        /// Fluent UI icon handle for dark mode
        /// </summary>
        public IntPtr HToolbarIconDarkMode;
    }

    /// <remarks>
    /// See <see cref="NppMsg.NPPM_ADDTOOLBARICON_DEPRECATED"/>
    /// </remarks>
    [Obsolete("Use ToolbarIconDarkMode instead", false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct ToolbarIcon
    {
        /// <inheritdoc cref="ToolbarIconDarkMode.HToolbarBmp"/>
        public IntPtr HToolbarBmp;
        /// <summary>Standard 32x32 icon handle</summary>
        public IntPtr HToolbarIcon;
    }

    /// <remarks>
    /// See <see cref="NppMsg.NPPM_SAVESESSION"/>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SessionInfo
    {
        /// <summary>
        /// Full path name of the session file to be saved
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.MAX_PATH)]
        public string SessionFilePathName;
        /// <summary>
        /// Size of the <see cref="Files"/> array (i.e., the number of files to be saved in the session)
        /// </summary>
        public int NumFiles;
        /// <summary>
        /// Array of file names (full path) to be saved in the session
        /// </summary>
        [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_LPWSTR)]
        public string[] Files;
    }

    /// <remarks>
    /// See <see cref="NppMsg.NPPM_MSGTOPLUGIN"/>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CommunicationInfo
    {
        /// <summary>
        /// Integer code defined by plugin Y, known by plugin X, identifying the message being sent.
        /// </summary>
        public int InternalMsg;
        /// <summary>
        /// The complete module name (with the extension '.dll') of caller (plugin X).
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.MAX_PATH)]
        public string SrcModuleName;
        /// <summary>
        /// Defined by the plugin; the information to be exchanged between X and Y.
        /// It's a void pointer so it should be defined by plugin Y and known by plugin X.
        /// </summary>
        public IntPtr Info;
    }

    /// <summary>
    /// The general type of a function defined by the plugin and callable by the host application.
    /// Delegates of this type are associated with a menu item by assigning them to the <see cref="FuncItem.PFunc"/> member
    /// of a <see cref="FuncItem"/> structure.
    /// </summary>
    public delegate void PluginFunc();
}
