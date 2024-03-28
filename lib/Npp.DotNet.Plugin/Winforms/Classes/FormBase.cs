/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Windows.Forms;
using static Npp.DotNet.Plugin.Win32;

namespace Npp.DotNet.Plugin.Winforms.Classes
{
    public partial class FormBase : Form
    {
        /// <summary>
        /// if true, this blocks the parent application until closed.<br></br>
        /// THIS IS ONLY TRUE OF POP-UP DIALOGS.
        /// </summary>
        public bool IsModal { get; private set; }

        /// <summary>
        /// if true, this form's default appearance is docked (attached) to the left, right, bottom, or top of the Notepad++ window.
        /// </summary>
        public bool IsDocking { get; private set; }

        /// <summary>
        /// indicates whether the form became visible for the first time<br></br>
        /// this is an unprincipled hack to deal with weirdness surrounding the opening of docking forms<br></br>
        /// since the Load and Shown events are suppressed on docking form startup.
        /// </summary>
        private bool IsLoaded { get; set; }

        private static WindowLongGetter _wndLongGetter = GetWindowLongPtr;
        private static WindowLongSetter _wndLongSetter = SetWindowLongPtr;

        /// <summary>
        /// superclass of all forms in the application.<br></br>
        /// Implements many useful handlers, and deals with some weird behaviors induced by interoperating with Notepad++.
        /// </summary>
        /// <param name="isModal">if true, this blocks the parent application until closed. THIS IS ONLY TRUE OF POP-UP DIALOGS</param>
        /// <param name="isDocking">if true, this form's default appearance is docked (attached) to the left, right, bottom, or top of the Notepad++ window.</param>
        public FormBase(bool isModal, bool isDocking)
        {
            InitializeComponent();
            IsModal = isModal;
            IsDocking = isDocking;
            Callbacks.RegisterFormIfModeless(this, isModal);
            if (IsDocking)
            {
                if (!Environment.Is64BitProcess) // we are 32-bit
                {
                    _wndLongGetter = GetWindowLong;
                    _wndLongSetter = SetWindowLong;
                }
            }
        }

        /// <summary>
        /// this is called every time the form's visibility changes,
        /// but it only does anything once, before the form is loaded for the first time.<br></br>
        /// This adds KeyUp, KeyDown, and KeyPress event handlers to all controls according to the recommendations in NppFormHelper.<br></br>
        /// It also styles the form using FormStyle.ApplyStyle
        /// </summary>
        public virtual void FormBase_VisibleChanged(object sender, EventArgs e)
        {
            if (IsLoaded || !Visible)
                return;
            IsLoaded = true;
            // we can't put this in the base constructor
            //     because it must be called *after* the subclass constructor adds all child controls
            //     and the base constructor must be called first (that's just how C# works)
            AddKeyUpDownPressHandlers(this);
        }

        /// <summary>
        /// This adds KeyUp, KeyDown, and KeyPress event handlers to all controls according to the recommendations in NppFormHelper.
        /// </summary>
        /// <param name="ctrl"></param>
        private void AddKeyUpDownPressHandlers(Control ctrl)
        {
            ctrl = ctrl ?? this;
            ctrl.KeyUp += (sender, e) => Callbacks.GenericKeyUpHandler(this, sender, e, IsModal);
            if (ctrl is TextBox tb)
                tb.KeyPress += Callbacks.TextBoxKeyPressHandler;
            else
                ctrl.KeyDown += Callbacks.GenericKeyDownHandler;
            if (ctrl.HasChildren)
            {
                foreach (Control child in ctrl.Controls)
                    AddKeyUpDownPressHandlers(child);
            }
        }

        private void FormBase_KeyUp(object sender, KeyEventArgs e)
        {
            Callbacks.GenericKeyUpHandler(this, sender, e, IsModal);
        }

        private void FormBase_KeyDown(object sender, KeyEventArgs e)
        {
            Callbacks.GenericKeyDownHandler(sender, e);
        }

        /// <summary>
        /// suppress the default response to the Tab key
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData.HasFlag(Keys.Tab)) // this covers Tab with or without modifiers
                return true;
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// this fixes a bug where Notepad++ can hang in the following situation:<br></br>
        /// 1. you are in a docking form<br></br>
        /// 2. you click a button that is a child of another control (e.g., a GroupBox)<br></br>
        /// 3. that button would cause a new form to appear<br></br>
        /// see https://github.com/BdR76/CSVLint/pull/88/commits
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (IsDocking)
            {
                switch (m.Msg)
                {
                    case WM_NOTIFY:
                        var nmdr = (TagNMHDR)m.GetLParam(typeof(TagNMHDR));

                        if (nmdr.hwndFrom == PluginData.NppData.NppHandle)
                        {
                            switch ((DockMgrMsg)(nmdr.code & 0xFFFFU))
                            {
                                case DockMgrMsg.DMN_DOCK:   // we are being docked
                                    break;
                                case DockMgrMsg.DMN_FLOAT:  // we are being _un_docked
                                    RemoveControlParent(this);
                                    break;
                                case DockMgrMsg.DMN_CLOSE:  // we are being closed
                                    break;
                            }
                        }
                        break;
                }
            }
            base.WndProc(ref m);
        }

        private void RemoveControlParent(Control parent)
        {
            if (parent.HasChildren)
            {
                long extAttrs = (long)_wndLongGetter(parent.Handle, GWL_EXSTYLE);
                if (WS_EX_CONTROLPARENT == (extAttrs & WS_EX_CONTROLPARENT))
                {
                    _wndLongSetter(parent.Handle, GWL_EXSTYLE, new IntPtr(extAttrs & ~WS_EX_CONTROLPARENT));
                }
                foreach (Control c in parent.Controls)
                {
                    RemoveControlParent(c);
                }
            }
        }
    }
}
