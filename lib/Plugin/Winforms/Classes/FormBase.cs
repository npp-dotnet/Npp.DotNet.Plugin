/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.ComponentModel; // https://learn.microsoft.com/dotnet/core/compatibility/windows-forms/9.0/security-analyzers#new-behavior
using System.Windows.Forms;
using static Npp.DotNet.Plugin.Win32;

namespace Npp.DotNet.Plugin.Winforms.Classes
{
    public partial class FormBase : Form
    {
        /// <summary>
        /// <see langword="true"/> if this form blocks the Notepad++ window until closed.
        /// </summary>
        [DefaultValue(false)]
        public bool IsModal { get; private set; }

        /// <summary>
        /// <see langword="true"/> if this form's default appearance is docked (attached) to the left, right, bottom, or top of the Notepad++ window.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDocking { get; private set; }

        /// <summary>
        /// <see langword="true"/> if this form is being shown for the first time.
        /// </summary>
        /// <remarks>
        /// This is an unprincipled hack to deal with weirdness surrounding the opening of docking forms,
        /// since the Load and Shown events are suppressed on docking form startup.
        /// </remarks>
        private bool IsLoaded { get; set; }

        private static WindowLongGetter WndLongGetter { get; set; } = GetWindowLongPtr;
        private static WindowLongSetter WndLongSetter { get; set; } = SetWindowLongPtr;

        /// <summary>
        /// Handles the <see cref="NppMsg.NPPN_DARKMODECHANGED"/> notification.
        /// </summary>
        /// <param name="isDark">Set this to <see langword="true"/> if dark mode has changed from disabled to enabled,
        /// or <see langword="false"/> if <em>vice versa</em>.</param>
        /// <remarks>
        /// Deriving classes should ensure all components have been initialized before calling this method.
        /// </remarks>
        public virtual void ToggleDarkMode(bool isDark) { }

        /// <summary>
        /// Ancestor class of all forms created by this plugin.
        /// </summary>
        /// <param name="isModal">See <see cref="IsModal"/>.</param>
        /// <param name="isDocking">See <see cref="IsDocking"/>.</param>
        /// <remarks>
        /// Implements many useful handlers, and deals with some weird behaviors induced by interoperating with Notepad++.
        /// </remarks>
        public FormBase(bool isModal, bool isDocking)
        {
            InitializeComponent();
            IsModal = isModal;
            IsDocking = isDocking;
            Callbacks.RegisterFormIfModeless(this, isModal);
            if (!Environment.Is64BitProcess)
            {
                WndLongGetter = GetWindowLong;
                WndLongSetter = SetWindowLong;
            }
        }

        /// <summary>
        /// Called when this form's visibility changes.
        /// </summary>
        /// <remarks>
        /// This is a "one-shot" handler; it exits early on every call but the first one.
        /// </remarks>
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
        /// Attaches the <see cref="Callbacks.GenericKeyUpHandler"/>, <see cref="Callbacks.GenericKeyDownHandler"/>,
        /// and <see cref="Callbacks.TextBoxKeyPressHandler"/> event handlers to the given <paramref name="ctrl"/>.
        /// </summary>
        /// <param name="ctrl">
        /// If <paramref name="ctrl"/> is an instance of <see cref="TextBox"/>, the key event handlers are attached to it.
        /// If it contains child controls, this method loops recursively over them.
        /// </param>
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
        /// Suppress the default response to the Tab key.
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData.HasFlag(Keys.Tab)) // this covers Tab with or without modifiers
                return true;
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Sets the <a href="https://learn.microsoft.com/windows/win32/winmsg/extended-window-styles#WS_EX_CONTROLPARENT">WS_EX_CONTROLPARENT</a> flag,
        /// e.g., whenever the Windows runtime wants to redraw this form.
        /// </summary>
        /// <remarks>
        /// See <see href="https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net/issues/17#issuecomment-683455467"/>.
        /// </remarks>
        protected void AddControlParent()
        {
            if (this.HasChildren)
            {
                long extAttrs = (long)WndLongGetter(this.Handle, GWL_EXSTYLE);
                if (WS_EX_CONTROLPARENT != (extAttrs & WS_EX_CONTROLPARENT))
                {
                    SetControlParent(this, extAttrs | WS_EX_CONTROLPARENT);
                }
            }
        }

        /// <summary>
        /// Clears the <a href="https://learn.microsoft.com/windows/win32/winmsg/extended-window-styles#WS_EX_CONTROLPARENT">WS_EX_CONTROLPARENT</a> flag,
        /// e.g., when the Docking Manager sends <see cref="DockMgrMsg.DMN_FLOAT"/> to this form's window procedure.
        /// </summary>
        /// <remarks>
        /// See <see href="https://sourceforge.net/p/notepad-plus/discussion/482781/thread/ab626469/#4458"/>.
        /// </remarks>
        protected void RemoveControlParent()
        {
            if (this.HasChildren)
            {
                long extAttrs = (long)WndLongGetter(this.Handle, GWL_EXSTYLE);
                if (WS_EX_CONTROLPARENT == (extAttrs & WS_EX_CONTROLPARENT))
                {
                    SetControlParent(this, extAttrs & ~WS_EX_CONTROLPARENT);
                }
            }
        }

        private static void SetControlParent(Control parent, long wsExMask)
        {
            if (parent.HasChildren)
            {
                WndLongSetter(parent.Handle, GWL_EXSTYLE, new IntPtr(wsExMask));
                foreach (Control c in parent.Controls)
                {
                    SetControlParent(c, wsExMask);
                }
            }
        }
    }
}
