/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.ComponentModel; // https://learn.microsoft.com/dotnet/core/compatibility/windows-forms/9.0/security-analyzers#new-behavior
using System.Windows.Forms;
using Npp.DotNet.Plugin.Winforms.Extensions;
using static Npp.DotNet.Plugin.Win32;

namespace Npp.DotNet.Plugin.Winforms.Classes
{
    enum DialogKind
    {
        PopUp, Docking, Modal
    }

    /// <summary>
    /// Encapsulates the minimum required logic to create a functioning plugin dialog.
    /// </summary>
    /// <remarks>
    /// Instances of this class should not be created directly.
    /// Create a <see cref="DockingForm"/> or a <see cref="ModalForm"/> instead.
    /// </remarks>
    public partial class FormBase : Form
    {
        /// <summary>
        /// <see langword="true"/> if this form blocks the Notepad++ window until closed.
        /// </summary>
        [DefaultValue(false)]
        protected internal bool IsModal { get => _dialogKind == DialogKind.Modal; }

        /// <summary>
        /// <see langword="true"/> if this form's default appearance is docked (attached) to the left, right, bottom, or top of the Notepad++ window.
        /// </summary>
        [DefaultValue(false)]
        protected internal bool IsDocking { get => _dialogKind == DialogKind.Docking; }

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
        private readonly DialogKind _dialogKind;

        /// <summary>
        /// Performs late-phase initialization that should be deferred until after a derived class has been constructed.
        /// </summary>
        private protected Action _postInit;

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
        /// Creates a new, non-modal instance of <see cref="FormBase"/> with no docking capability.
        /// </summary>
        /// <remarks>
        /// This constructor is provided only to satisfy the Visual Studio Designer.
        /// </remarks>
        [Obsolete("Your plugin dialog should inherit from `DockingForm` or `ModalForm` instead")]
        public FormBase() : this(DialogKind.PopUp) { }

        /// <summary>
        /// Ancestor class of all forms created by this plugin.
        /// </summary>
        /// <param name="kind">The type of dialog this form represents.</param>
        /// <remarks>
        /// Implements many useful handlers, and deals with some weird behaviors induced by interoperating with Notepad++.
        /// </remarks>
        private protected FormBase(DialogKind kind = default)
        {
            _dialogKind = kind;
            InitializeComponent();
            RegisterFormIfModeless();
            if (!Environment.Is64BitProcess)
            {
                WndLongGetter = GetWindowLong;
                WndLongSetter = SetWindowLong;
            }
        }

        /// <summary>
        /// Attaches the key event handlers provided by <see cref="Callbacks"/> to this form, and, if applicable, to
        /// each of its child components, recursively.
        /// </summary>
        /// <remarks>
        /// This is a "one-shot" handler; it exits early on every call but the first one.
        /// </remarks>
        protected virtual void AttachEventHandlers()
        {
            if (IsLoaded || !Visible)
                return;
            IsLoaded = true;
            AddKeyUpDownPressHandlers(this);
            _postInit?.Invoke();
        }

        /// <summary>
        /// When this form is initialized, and it's a modeless dialog (i.e., <see cref="IsModal"/> is
        /// <see langword="false"/>), sends the <see cref="NppMsg.NPPM_MODELESSDIALOG"/> message with
        /// <see cref="NppMsg.MODELESSDIALOGADD"/> to register this form.
        /// </summary>
        private void RegisterFormIfModeless()
        {
            if (!IsModal)
                SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_MODELESSDIALOG, (uint)NppMsg.MODELESSDIALOGADD, Handle);
        }

        /// <summary>
        /// When disposing this form, and it's a modeless dialog (i.e., <see cref="IsModal"/> is
        /// <see langword="false"/>), sends the <see cref="NppMsg.NPPM_MODELESSDIALOG"/> message with
        /// <see cref="NppMsg.MODELESSDIALOGREMOVE"/> to <em>un</em>register this form.
        /// </summary>
        private void UnregisterFormIfModeless()
        {
            if (!IsDisposed && !IsModal)
                SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_MODELESSDIALOG, (uint)NppMsg.MODELESSDIALOGREMOVE, Handle);
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
            ctrl.KeyUp += this.GenericKeyUpHandler;
            if (ctrl is TextBox tb)
                tb.KeyPress += this.TextBoxKeyPressHandler;
            else
                ctrl.KeyDown += this.GenericKeyDownHandler;
            if (ctrl.HasChildren)
            {
                foreach (Control child in ctrl.Controls)
                    AddKeyUpDownPressHandlers(child);
            }
        }

        private void FormBase_KeyUp(object sender, KeyEventArgs e)
        {
            this.GenericKeyUpHandler(sender, e);
        }

        private void FormBase_KeyDown(object sender, KeyEventArgs e)
        {
            this.GenericKeyDownHandler(sender, e);
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
                SetControlParent(this, extAttrs | WS_EX_CONTROLPARENT);
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
                SetControlParent(this, extAttrs & ~WS_EX_CONTROLPARENT);
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
