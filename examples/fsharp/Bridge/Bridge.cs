/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Npp.DotNet.Plugin.FSharp.Demo
{
    internal sealed class Bridge
    {
        #region "Call the plugin's static initializer"
        private static readonly IDotNetPlugin Instance;
        static Bridge()
        {
            Instance = Main.Instance;
        }
        #endregion

        #region "Expose interface methods to unmanaged callers (like notepad++.exe)"
        /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.OnSetInfo" />
        [UnmanagedCallersOnly(EntryPoint = "setInfo", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal unsafe static void SetInfo(NppData* notepadPlusData)
        {
            PluginData.NppData = *notepadPlusData;
            Instance.OnSetInfo();
        }

        /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.OnBeNotified" />
        [UnmanagedCallersOnly(EntryPoint = "beNotified", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal unsafe static void BeNotified(ScNotification* notification)
        {
            Instance.OnBeNotified(*notification);
        }

        /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.OnMessageProc" />
        [UnmanagedCallersOnly(EntryPoint = "messageProc", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static NativeBool MessageProc(uint msg, UIntPtr wParam, IntPtr lParam)
        {
            return Instance.OnMessageProc(msg, wParam, lParam);
        }

        /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.OnGetFuncsArray" />
        [UnmanagedCallersOnly(EntryPoint = "getFuncsArray", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static IntPtr GetFuncsArray(IntPtr nbF) => IDotNetPlugin.OnGetFuncsArray(nbF);

        /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.OnGetName" />
        [UnmanagedCallersOnly(EntryPoint = "getName", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static IntPtr GetName() => IDotNetPlugin.OnGetName();

        /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.OnIsUnicode" />
        [UnmanagedCallersOnly(EntryPoint = "isUnicode", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static NativeBool IsUnicode() => IDotNetPlugin.OnIsUnicode();
        #endregion
    }
}
