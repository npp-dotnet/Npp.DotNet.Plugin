/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Runtime.InteropServices;

namespace Npp.DotNet.Plugin
{
    /// <summary>
    /// Provides managed wrappers around the core Notepad++ plugin APIs.
    /// Plugins can implement external lexers by extending the <see cref="DotNetPlugin"/> base class.
    /// </summary>
    /// <remarks>
    /// For a guide to the external lexer API, see <see href="https://community.notepad-plus-plus.org/post/76117"/>
    /// </remarks>
    internal interface IDotNetPlugin
    {
        /// <inheritdoc cref="DotNetPlugin.OnSetInfo"/>
        void OnSetInfo();
        /// <inheritdoc cref="DotNetPlugin.OnBeNotified"/>
        void OnBeNotified(ScNotification notification);
        /// <inheritdoc cref="DotNetPlugin.OnMessageProc"/>
        NativeBool OnMessageProc(uint msg, UIntPtr wParam, IntPtr lParam);
    }

    /// <summary>
    /// Default implementation of the <see cref="IDotNetPlugin"/> interface.
    /// </summary>
    public class DotNetPlugin : IDotNetPlugin
    {
        /// <summary>
        /// Called by the unmanaged <c>setInfo</c> Notepad++ API function.<br/>
        /// All setup logic that needs a valid handle to the host application window should go into this method.
        /// </summary>
        /// <remarks>Overridable.</remarks>
        public virtual void OnSetInfo() { }
        /// <summary>
        /// Called by the unmanaged <c>beNotified</c> Notepad++ API function.<br/>
        /// Callbacks for editor events and notifications go here.
        /// </summary>
        /// <remarks>Overridable.</remarks>
        public virtual void OnBeNotified(ScNotification notification) { }
        /// <summary>
        /// Called by the unmanaged <c>messageProc</c> Notepad++ API function.<br/>
        /// Callbacks for Win32 window messages go here. Some editor notifications may be sent to it as well.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> to forward the message to the system for further processing;
        /// <see langword="false"/> to signal that this method will handle the message instead.
        /// </returns>
        /// <remarks>Overridable.</remarks>
        public virtual NativeBool OnMessageProc(uint msg, UIntPtr wParam, IntPtr lParam) => Win32.TRUE;
        /// <summary>
        /// Called by the unmanaged <c>getFuncsArray</c> Notepad++ API function.<br/>
        /// </summary>
        /// <param name="nbF">The number of commands provided by this plugin.</param>
        /// <returns>A pointer to a managed <see cref="PluginFuncArray"/>.</returns>
        /// <remarks>
        /// A positive non-zero value must be assigned to <paramref name="nbF"/>, and the return value
        /// must not be a <c>NULL</c> pointer, a.k.a <see cref="IntPtr.Zero"/>.<br/><br/>
        /// Deriving classes <b>should not </b> override this.
        /// </remarks>
        public static IntPtr OnGetFuncsArray(IntPtr nbF)
        {
            Marshal.WriteInt32(nbF, (PluginData.FuncItems?.Items.Count).GetValueOrDefault());
            return (PluginData.FuncItems?.NativePointer).GetValueOrDefault(IntPtr.Zero);
        }
        /// <summary>
        /// Called by the unmanaged <c>getName</c> Notepad++ API function.<br/>
        /// </summary>
        /// <returns>The allocated string pointer held by <see cref="PluginData.PszPluginName"/>.</returns>
        /// <remarks>
        /// The return value must not be a <c>NULL</c> pointer, a.k.a <see cref="IntPtr.Zero"/>.<br/><br/>
        /// Deriving classes <b>should not </b> override this.
        /// </remarks>
        public static IntPtr OnGetName() => PluginData.PluginNamePtr;
        /// <summary>
        /// Called by the unmanaged <c>isUnicode</c> API function.<br/>
        /// </summary>
        /// <returns><see langword="true"/></returns>
        /// <remarks>Deriving classes <b>should not </b> override this.</remarks>
        public static NativeBool OnIsUnicode() => Win32.TRUE;
    }
}
