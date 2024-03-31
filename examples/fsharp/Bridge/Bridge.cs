/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Npp.DotNet.Plugin.DotNetPlugin;

namespace Npp.DotNet.Plugin.FSharp.Demo
{
    internal sealed class Bridge
    {
        #region "Call the plugin's static initializer"
        private static readonly Main Instance;
        static Bridge()
        {
            Instance = Main.Instance;
        }
        #endregion

        #region "Expose interface methods to unmanaged callers (like notepad++.exe)"
        /// <inheritdoc cref="Npp.DotNet.Plugin.DotNetPlugin.OnSetInfo" />
        [UnmanagedCallersOnly(EntryPoint = "setInfo", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal unsafe static void SetInfo(NppData* notepadPlusData)
        {
            PluginData.NppData = *notepadPlusData;
            Instance.OnSetInfo();
        }

        /// <inheritdoc cref="Npp.DotNet.Plugin.DotNetPlugin.OnBeNotified" />
        [UnmanagedCallersOnly(EntryPoint = "beNotified", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal unsafe static void BeNotified(ScNotification* notification)
        {
            Instance.OnBeNotified(*notification);
        }

        /// <inheritdoc cref="Npp.DotNet.Plugin.DotNetPlugin.OnMessageProc" />
        [UnmanagedCallersOnly(EntryPoint = "messageProc", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static NativeBool MessageProc(uint msg, UIntPtr wParam, IntPtr lParam)
        {
            return Instance.OnMessageProc(msg, wParam, lParam);
        }

        /// <inheritdoc cref="Npp.DotNet.Plugin.DotNetPlugin.GetFuncsArray" />
        [UnmanagedCallersOnly(EntryPoint = "getFuncsArray", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static IntPtr GetFuncsArray(IntPtr nbF) => OnGetFuncsArray(nbF);

        /// <inheritdoc cref="Npp.DotNet.Plugin.DotNetPlugin.GetName" />
        [UnmanagedCallersOnly(EntryPoint = "getName", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static IntPtr GetName() => OnGetName();

        /// <inheritdoc cref="Npp.DotNet.Plugin.DotNetPlugin.IsUnicode" />
        [UnmanagedCallersOnly(EntryPoint = "isUnicode", CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static NativeBool IsUnicode() => OnIsUnicode();
        #endregion
    }
}
