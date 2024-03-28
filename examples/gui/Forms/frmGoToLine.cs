/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *                         2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Npp.DotNet.Plugin;
using Npp.DotNet.Plugin.Winforms.Classes;

namespace Kbg.Demo.Namespace
{
    partial class frmGoToLine : FormBase
    {
        private readonly IScintillaGateway editor;

        public frmGoToLine(IScintillaGateway editor) : base(false, true)
        {
            this.editor = editor;
            InitializeComponent();
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

        private void frmGoToLine_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == Keys.Return) || (e.Alt && (e.KeyCode == Keys.G)))
            {
                button1.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyData == Keys.Escape)
            {
                editor.GrabFocus();
            }
            else if (e.KeyCode == Keys.Tab)
            {
                Control next = GetNextControl((Control)sender, !e.Shift)!;
                while ((next == null) || (!next.TabStop)) next = GetNextControl(next, !e.Shift)!;
                next.Focus();
                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar)
                && (e.KeyChar != '\b')
                && (e.KeyChar != '\t'))
                e.Handled = true;
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
