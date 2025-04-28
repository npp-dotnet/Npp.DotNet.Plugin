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
    /// Plugins can implement external lexers by extending this interface.
    /// </summary>
    /// <remarks>
    /// For a guide to the external lexer API, see <see href="https://community.notepad-plus-plus.org/post/76117"/>
    /// </remarks>
    public interface IDotNetPlugin
    {
        /// <summary>
        /// Called by the unmanaged <c>setInfo</c> Notepad++ API function.<br/>
        /// All setup logic that needs a valid handle to the host application window should go into this method.
        /// </summary>
        void OnSetInfo();
        /// <summary>
        /// Called by the unmanaged <c>beNotified</c> Notepad++ API function.<br/>
        /// Callbacks for editor events and notifications go here.
        /// </summary>
        void OnBeNotified(ScNotification notification);
        /// <summary>
        /// Called by the unmanaged <c>messageProc</c> Notepad++ API function.<br/>
        /// Callbacks for Win32 window messages go here. Some editor notifications may be sent to it as well.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> to forward the message to the system for further processing;
        /// <see langword="false"/> to signal that this method will handle the message instead.
        /// </returns>
        NativeBool OnMessageProc(uint msg, UIntPtr wParam, IntPtr lParam);
        // Interfaces can implement default methods since C# 8.0
        // https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-8.0/default-interface-methods
#if NETCOREAPP3_0_OR_GREATER
        /// <summary>
        /// Called by the unmanaged <c>getFuncsArray</c> Notepad++ API function.<br/>
        /// </summary>
        /// <param name="nbF">The number of commands provided by this plugin.</param>
        /// <returns>A pointer to a managed <see cref="PluginFuncArray"/>.</returns>
        /// <remarks>
        /// A positive non-zero value must be assigned to <paramref name="nbF"/>, and the return value
        /// must not be <see cref="Win32.NULL"/>.
        /// </remarks>
        static IntPtr OnGetFuncsArray(IntPtr nbF)
        {
            Marshal.WriteInt32(nbF, (PluginData.FuncItems?.Items.Count).GetValueOrDefault());
            return (PluginData.FuncItems?.NativePointer).GetValueOrDefault(IntPtr.Zero);
        }
        /// <summary>
        /// Called by the unmanaged <c>getName</c> Notepad++ API function.<br/>
        /// </summary>
        /// <returns>The allocated string pointer held by <see cref="PluginData.PluginNamePtr"/>.</returns>
        /// <remarks>
        /// The return value must not be <see cref="Win32.NULL"/>.
        /// </remarks>
        static IntPtr OnGetName() => PluginData.PluginNamePtr;
        /// <summary>
        /// Called by the unmanaged <c>isUnicode</c> API function.<br/>
        /// </summary>
        /// <returns><see langword="true"/></returns>
        static NativeBool OnIsUnicode() => Win32.TRUE;
#endif
    }

#if !NETCOREAPP
    /// <summary>
    /// Default implementations of Notepad++ plugin APIs that should be identical for every plugin.
    /// </summary>
    /// <remarks>
    /// Classes implementing <see cref="IDotNetPlugin"/> <b>should not</b> override these.
    /// </remarks>
    public static class IDotNetPluginImpl
    {
        /// <summary>
        /// Called by the unmanaged <c>getFuncsArray</c> Notepad++ API function.<br/>
        /// </summary>
        /// <param name="iface">The <see cref="IDotNetPlugin"/> interface type.</param>
        /// <param name="nbF">The number of commands provided by this plugin.</param>
        /// <returns>A pointer to a managed <see cref="PluginFuncArray"/>.</returns>
        /// <remarks>
        /// A positive non-zero value must be assigned to <paramref name="nbF"/>, and the return value
        /// must not be <see cref="Win32.NULL"/>.
        /// </remarks>
        public static IntPtr OnGetFuncsArray(this IDotNetPlugin iface, IntPtr nbF)
        {
            Marshal.WriteInt32(nbF, (PluginData.FuncItems?.Items.Count).GetValueOrDefault());
            return (PluginData.FuncItems?.NativePointer).GetValueOrDefault(IntPtr.Zero);
        }
        /// <summary>
        /// Called by the unmanaged <c>getName</c> Notepad++ API function.<br/>
        /// </summary>
        /// <returns>The allocated string pointer held by <see cref="PluginData.PluginNamePtr"/>.</returns>
        /// <remarks>
        /// The return value must not be <see cref="Win32.NULL"/>.
        /// </remarks>
        public static IntPtr OnGetName(this IDotNetPlugin iface) => PluginData.PluginNamePtr;
        /// <summary>
        /// Called by the unmanaged <c>isUnicode</c> API function.<br/>
        /// </summary>
        /// <returns><see langword="true"/></returns>
        public static NativeBool OnIsUnicode(this IDotNetPlugin iface) => Win32.TRUE;
    }
#endif
}
