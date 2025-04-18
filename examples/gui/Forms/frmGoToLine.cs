﻿/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *                         2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Npp.DotNet.Plugin;
using Npp.DotNet.Plugin.Winforms;
using Npp.DotNet.Plugin.Winforms.Classes;
using System.Runtime.InteropServices;
using static Npp.DotNet.Plugin.Win32;
using static Npp.DotNet.Plugin.Winforms.DarkMode;

namespace Kbg.Demo.Namespace
{
    partial class frmGoToLine : FormBase
    {
        private readonly IScintillaGateway editor;

        public frmGoToLine(IScintillaGateway editor) : base(false, true)
        {
            this.editor = editor;
            InitializeComponent();
            ToggleDarkMode(PluginData.Notepad.IsDarkModeEnabled());
        }

        public override void ToggleDarkMode(bool isDark)
        {
            if (isDark)
            {
                DarkModeColors theme = new();
                label1.BackColor = theme.PureBackground;
                label1.ForeColor = theme.Text;
                button1.BackColor = theme.SofterBackground;
                button1.ForeColor = theme.Text;
            }
            else
            {
                label1.BackColor = Label.DefaultBackColor;
                label1.ForeColor = Label.DefaultForeColor;
                button1.BackColor = Color.FromKnownColor(KnownColor.ButtonFace);
                button1.ForeColor = Button.DefaultForeColor;
            }
        }

        /// <summary>
        /// Intercepts the default WndProc to prevent an infinite redraw loop when the form undocks.
        /// </summary>
        /// <param name="wmNotifyMsg">A window notification message structure.</param>
        /// <remarks>See <see href="https://sourceforge.net/p/notepad-plus/discussion/482781/thread/ab626469/#4458"/></remarks>
        protected override void WndProc(ref Message wmNotifyMsg)
        {
            switch (wmNotifyMsg.Msg)
            {
                case WM_NOTIFY:
                    TagNMHDR nmdr = Marshal.PtrToStructure<TagNMHDR>(wmNotifyMsg.LParam)!;

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
                                break;
                        }
                    }
                    break;
            }
            base.WndProc(ref wmNotifyMsg);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int line;
            if (!int.TryParse(textBox1.Text, out line))
                return;
            editor.EnsureVisible(line - 1);
            editor.GotoLine(line - 1);
            editor.GrabFocus();
        }

        private void FrmGoToLineKeyUp(object sender, KeyEventArgs e)
        {
            Callbacks.GenericKeyUpHandler(this, sender, e, this.IsModal);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Callbacks.TextBoxKeyPressHandler(sender, e);
        }

        void FrmGoToLineVisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                                  (uint)PluginData.FuncItems.Items[Main.idFrmGotToLine].CmdID, 0);
            }
        }
    }
}
