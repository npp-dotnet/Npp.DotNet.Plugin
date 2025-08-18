/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Windows.Forms;
using Npp.DotNet.Plugin.Winforms.Classes;

namespace Npp.DotNet.Plugin.Winforms.Extensions
{
    /// <summary>
    /// Extension methods available to all child classes of <see cref="FormBase"/>.
    /// </summary>
    public static class Callbacks
    {
        /// <summary>
        /// CALL THIS IN YOUR KeyDown HANDLER FOR ALL CONTROLS *except TextBoxes*<br></br>
        /// suppress annoying ding when user hits escape, enter, tab, or space
        /// </summary>
        public static void GenericKeyDownHandler(this FormBase form, object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Space)
                e.SuppressKeyPress = true;
        }

        /// <summary>
        /// CALL THIS IN YOUR KeyDown HANDLER FOR ALL TextBoxes and ComboBoxes<br></br>
        /// suppress annoying ding when user hits tab
        /// </summary>
        public static void TextBoxKeyPressHandler(this FormBase form, object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
                e.Handled = true;
        }

        /// <summary>
        /// CALL THIS IN YOUR KeyUp HANDLER FOR ALL CONTROLS (but only add to the form itself *IF NOT isModal*)<br></br>
        /// Enter presses button,<br></br>
        /// escape focuses editor (or closes if isModal),<br></br>
        /// Ctrl+V pastes text into text boxes and combo boxes<br></br>
        /// if isModal:<br></br>
        /// - tab goes through controls,<br></br>
        /// - shift-tab -> go through controls backward<br></br>
        /// </summary>
        /// <param name="form"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void GenericKeyUpHandler(this FormBase form, object sender, KeyEventArgs e)
        {
            // enter presses button
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (sender is Button btn)
                {
                    // Enter has the same effect as clicking a selected button
                    btn.PerformClick();
                }
                else
                    PressEnterInTextBoxHandler(form, sender);
            }
            // Escape ->
            //     * if this.IsModal (meaning this is a pop-up dialog), close this.
            //     * otherwise, focus the editor component.
            else if (e.KeyData == Keys.Escape)
            {
                if (form.IsModal)
                    form.Close();
                else
                    PluginData.Editor.GrabFocus();
            }
            // Tab -> go through controls, Shift+Tab -> go through controls backward
            else if (e.KeyCode == Keys.Tab && form.IsModal)
            {
                GenericTabNavigationHandler(form, sender, e);
            }
        }

        /// <summary>
        /// CALL THIS METHOD IN A KeyUp HANDLER, *UNLESS USING GenericKeyUpHandler ABOVE*<br></br>
        /// Tab -> go through controls, Shift+Tab -> go through controls backward.<br></br>
        /// Ignores invisible or disabled controls.
        /// </summary>
        public static void GenericTabNavigationHandler(this FormBase form, object sender, KeyEventArgs e)
        {
            if (sender is TextBox tb && tb.Parent is ListBox)
                return; // ComboBoxes are secretly two controls in one (see https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox?view=windowsdesktop-8.0)
                        // this event fires twice for a CombobBox because of this, so we need to suppress the extra one this way
            Control next = form.GetNextControl((Control)sender, !e.Shift);
            while (next == null || !next.TabStop || !next.Visible || !next.Enabled)
                next = form.GetNextControl(next, !e.Shift);
            next.Focus();
            e.Handled = true;
        }

        /// <summary>
        /// NPPM_MODELESSDIALOG consumes the KeyDown and KeyPress events for the Enter key,<br></br>
        /// so our KeyUp handler needs to simulate pressing enter to add a new line in a multiline text box.<br></br>
        /// Note that this does not fully repair the functionality of the Enter key in a multiline text box,
        /// because only one newline can be created for a single keypress of Enter, no matter how long the key is held down.
        /// </summary>
        /// <param name="form">the parent form</param>
        /// <param name="sender">the text box that sent the message</param>
        static void PressEnterInTextBoxHandler(this FormBase form, object sender)
        {

            if (!form.IsModal && sender is TextBox tb && tb.Multiline)
            {
                int selstart = tb.SelectionStart;
                tb.SelectedText = "";
                string text = tb.Text;
                tb.Text = text.Substring(0, selstart) + "\r\n" + text.Substring(selstart);
                tb.SelectionStart = selstart + 2; // after the inserted newline
                tb.SelectionLength = 0;
                tb.ScrollToCaret();
            }
        }
    }
}
