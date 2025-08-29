/*
 * SPDX-FileCopyrightText: 2025 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using static Npp.DotNet.Plugin.Win32;

namespace Npp.DotNet.Plugin.Winforms.Classes
{
    /// <summary>
    /// A dialog associated with a plugin command and registered with the Notepad++ Docking Manager.
    /// </summary>
    public class DockingForm : FormBase
    {
        private NppTbData _toolBarData = default;
        private readonly IntPtr _toolBarDataPtr = default;
        private int _disposed = 0;

        /// <summary>
        /// Gets the <see cref="NppTbData"/> instance associated with this <see cref="DockingForm"/>.
        /// </summary>
        protected NppTbData ToolBarData { get => _toolBarData; }

        /// <summary>
        /// Creates a new, uninitialized <see cref="DockingForm"/> with no toolbar data.
        /// </summary>
        /// <remarks>
        /// This constructor is provided only to satisfy the Visual Studio Designer.
        /// </remarks>
        [Obsolete("Use DockingForm(System.Int32, System.String, [System.String], [System.String], [System.Drawing.Icon], [Npp.DotNet.Plugin.Winforms.NppTbMsg]) instead")]
        public DockingForm() : base(DialogKind.Docking) { }

        /// <summary>
        /// Creates a new <see cref="DockingForm"/>, setting the item index of the associated plugin
        /// command to <paramref name="funcIndex"/>, and the plugin DLL name to <paramref name="dllName"/>.
        /// <para>
        /// If provided, the window title is set to <paramref name="caption"/>, additional title bar text is set to
        /// <paramref name="subtitle"/>, the window icon is set to <paramref name="tabIcon"/>, and the dialog window
        /// attributes are set according to <paramref name="flags"/>.
        /// </para>
        /// </summary>
        /// <param name="funcIndex">
        /// Index within <see cref="PluginData.FuncItems"/> of the plugin command that will launch the dialog.
        /// </param>
        /// <param name="dllName">
        /// Name of the plugin's DLL module, including the &quot;.dll&quot; file extension.
        /// </param>
        /// <param name="caption">
        /// Text to display in the dialog's title bar.
        /// </param>
        /// <param name="subtitle">
        /// Additional text for the dialog's title bar, displayed after <paramref name="caption"/>.
        /// </param>
        /// <param name="tabIcon">
        /// Icon to display on the dialog's tab (when hidden by other docking forms).
        /// </param>
        /// <param name="flags">
        /// Dialog window attribute flags, used to initialize the <see cref="NppTbData.UMask"/> field of the
        /// dialog's internal toolbar data.
        /// <para>Providing a valid icon will cause the <see cref="NppTbMsg.DWS_ICONTAB "/> and
        /// <see cref="NppTbMsg.DWS_ICONBAR"/> flags to be set.</para>
        /// <para> Passing a non-empty string for <paramref name="subtitle"/> will cause the
        /// <see cref="NppTbMsg.DWS_ADDINFO "/> flag to be set.</para>
        /// </param>
        /// <remarks>
        /// The plugin's DLL name will be reused as the window title if no <paramref name="caption"/> is provided.
        /// </remarks>
        public DockingForm(
            int funcIndex,
            string dllName,
            string caption = null,
            string subtitle = null,
            Icon tabIcon = null,
            NppTbMsg flags = NppTbMsg.DWS_DF_CONT_LEFT)
            : base(DialogKind.Docking)
        {
            _toolBarData = new NppTbData()
            {
                HClient = this.Handle,
                DlgID = funcIndex,
                PszModuleName = dllName,
                PszName = caption,
                PszAddInfo = subtitle,
                HIconTab = tabIcon?.Handle ?? IntPtr.Zero,
                UMask = flags
            };

            if (string.IsNullOrEmpty(_toolBarData.PszName))
                _toolBarData.PszName = _toolBarData.PszModuleName.Replace(".dll", "");
            if (_toolBarData.HIconTab != IntPtr.Zero)
                _toolBarData.UMask |= NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
            if (!string.IsNullOrEmpty(_toolBarData.PszAddInfo))
                _toolBarData.UMask |= NppTbMsg.DWS_ADDINFO;

            try
            {
                _toolBarDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf(_toolBarData));
                Marshal.StructureToPtr(_toolBarData, _toolBarDataPtr, false);
                SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _toolBarDataPtr);
                Utils.CheckMenuItem(_toolBarData.DlgID, true);
            }
            finally { }
        }

        /// <summary>
        /// Hides this <see cref="DockingForm"/> and unchecks the associated plugin menu item.
        /// </summary>
        public void HideDockingForm()
        {
            SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, Handle);
            Utils.CheckMenuItem(_toolBarData.DlgID, false);
        }

        /// <summary>
        /// Makes this <see cref="DockingForm"/> visible and checks the associated plugin menu item.
        /// </summary>
        public void ShowDockingForm()
        {
            SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, Handle);
            Utils.CheckMenuItem(_toolBarData.DlgID, true);
        }

        /// <summary>
        /// Intercepts the default window procedure to prevent an infinite redraw loop when the form undocks.
        /// </summary>
        /// <param name="wmNotifyMsg">A window notification message structure.</param>
        /// <remarks>See <see href="https://sourceforge.net/p/notepad-plus/discussion/482781/thread/ab626469/#4458"/></remarks>
        protected override void WndProc(ref Message wmNotifyMsg)
        {
            switch (wmNotifyMsg.Msg)
            {
                case WM_NOTIFY:
                    TagNMHDR nmdr = Marshal.PtrToStructure<TagNMHDR>(wmNotifyMsg.LParam);
                    if (nmdr.HwndFrom == PluginData.NppData.NppHandle)
                    {
                        switch ((DockMgrMsg)(nmdr.Code & 0xFFFFU))
                        {
                            case DockMgrMsg.DMN_DOCK:
                                break;
                            case DockMgrMsg.DMN_FLOAT:
                                RemoveControlParent();
                                break;
                            case DockMgrMsg.DMN_CLOSE:
                                HideDockingForm();
                                break;
                        }
                    }
                    break;
            }
            base.WndProc(ref wmNotifyMsg);
        }

        /// <summary>
        /// Overrides <see cref="Control.Dispose(bool)"/> to perform thread-safe deallocation of unmanaged private data.
        /// </summary>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/dotnet/standard/garbage-collection/implementing-dispose"/>
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            if (System.Threading.Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                if (_toolBarDataPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(_toolBarDataPtr);
            }
            base.Dispose(disposing);
        }

        ~DockingForm() => Dispose(false);
    }
}
