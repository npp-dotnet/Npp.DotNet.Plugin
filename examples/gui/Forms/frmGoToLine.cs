/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *                         2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Npp.DotNet.Plugin;
using Npp.DotNet.Plugin.Winforms.Classes;
using Npp.DotNet.Plugin.Winforms.Extensions;
using static Npp.DotNet.Plugin.Winforms.DarkMode;

namespace Kbg.Demo.Namespace
{
    partial class frmGoToLine : DockingForm
    {
        private readonly IScintillaGateway editor;

        public frmGoToLine(IScintillaGateway editor, int dlgID, string pluginName, Icon frmIcon)
            : base(dlgID, pluginName, "Go To Line #", null, frmIcon)
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
            this.GenericKeyUpHandler(sender, e);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.TextBoxKeyPressHandler(sender, e);
        }

        void FrmGoToLineVisibleChanged(object sender, EventArgs e)
        {
            Utils.CheckMenuItem(ToolBarData.DlgID, Visible);
        }
    }
}
