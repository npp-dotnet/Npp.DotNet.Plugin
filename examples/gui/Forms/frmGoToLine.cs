/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *                         2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using Npp.DotNet.Plugin;
using Npp.DotNet.Plugin.Winforms;
using Npp.DotNet.Plugin.Winforms.Classes;
using Npp.DotNet.Plugin.Winforms.Extensions;
using static Npp.DotNet.Plugin.Winforms.DarkMode;

namespace Kbg.Demo.Namespace
{
    partial class frmGoToLine : DockingForm
    {
        private readonly IScintillaGateway editor;

        public frmGoToLine(IScintillaGateway editor, int dlgID, string pluginName, Icon frmIcon)
            : base(dlgID, pluginName, "Go To Line #", null, frmIcon, NppTbMsg.DWS_DF_CONT_RIGHT)
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
                if (label1 is not null)
                {
                    label1.BackColor = theme.PureBackground;
                    label1.ForeColor = theme.Text;
                }
                if (button1 is not null)
                {
                    button1.BackColor = theme.SofterBackground;
                    button1.ForeColor = theme.Text;
                }
                return;
            }
            if (label1 is not null)
            {
                label1.BackColor = Label.DefaultBackColor;
                label1.ForeColor = Label.DefaultForeColor;
            }
            if (button1 is not null)
            {
                button1.BackColor = Color.FromKnownColor(KnownColor.ButtonFace);
                button1.ForeColor = Button.DefaultForeColor;
            }
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            int line;
            if (!int.TryParse(textBox1?.Text, out line))
                return;
            editor.EnsureVisible(line - 1);
            editor.GotoLine(line - 1);
            editor.GrabFocus();
        }

        private void FrmGoToLineKeyUp(object? sender, KeyEventArgs e)
        {
            this.GenericKeyUpHandler(sender, e);
        }

        private void textBox1_KeyPress(object? sender, KeyPressEventArgs e)
        {
            this.TextBoxKeyPressHandler(sender, e);
        }

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.button1 = new Button();
            this.textBox1 = new TextBox();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Go to line:";
            //
            // button1
            //
            this.button1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.button1.Location = new Point(15, 32);
            this.button1.Name = "button1";
            this.button1.Size = new Size(107, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "&Go";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.button1.KeyUp += new KeyEventHandler(this.FrmGoToLineKeyUp);
            //
            // textBox1
            //
            this.textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.textBox1.Location = new Point(73, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Size(49, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.KeyUp += new KeyEventHandler(this.FrmGoToLineKeyUp);
            this.textBox1.KeyPress += new KeyPressEventHandler(this.textBox1_KeyPress);
            //
            // frmGoToLine
            //
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(139, 70);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "frmGoToLine";
            this.Text = "NppDockableForm";
            this.KeyUp += new KeyEventHandler(this.FrmGoToLineKeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label? label1;
        private Button? button1;
        internal TextBox? textBox1;
    }
}
