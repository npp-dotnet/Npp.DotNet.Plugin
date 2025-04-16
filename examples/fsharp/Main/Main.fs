(*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 * SPDX-License-Identifier: Apache-2.0
 *)

namespace Npp.DotNet.Plugin.FSharp.Demo

open Microsoft.FSharp.Core.LanguagePrimitives
open System.Runtime.InteropServices
open Npp.DotNet.Plugin

open type System.Diagnostics.FileVersionInfo
open type Npp.DotNet.Plugin.Win32

/// <summary>
/// Extends <see cref="Npp.DotNet.Plugin.DotNetPlugin"/>.
/// </summary>
type Main =
    inherit DotNetPlugin

    static let pluginName = "F# Demo Plugin\000"

    /// <summary>
    /// Use this to initialize any data your plugin needs when starting up.
    /// At the very least, assign a unique name to the static <see cref="PluginData.PluginNamePtr"/> property.
    /// Otherwise <see cref="PluginData.DefaultPluginName"/> will be used.
    /// </summary>
    /// <remarks>
    /// This must be <see langword="static"/> so that initialization is automatic when the <see cref="Instance"/>
    /// member is accessed by the unmanaged methods of the <see cref="Bridge"/> class.
    /// </remarks>
    static let instance =
        PluginData.PluginNamePtr <- Marshal.StringToHGlobalUni(pluginName)
        new Main()

    static member val Instance = instance

    override __.OnSetInfo() =
        /// <summary>
        /// Creates a new buffer and inserts text into it.
        /// </summary>
        let helloNpp () =
            PluginData.Notepad.FileNew()
            PluginData.Editor.SetText("Hello, Notepad++ ... from F#!")

        /// <summary>
        /// Shows the plugin's version number in a system dialog.
        /// </summary>
        let displayInfo () =
            let mutable version = "1.0.0.0"
            try
                let assemblyName = typeof<Main>.Namespace
                version <-
                    GetVersionInfo(
                        System.IO.Path.Combine(
                            PluginData.Notepad.GetPluginsHomePath(), assemblyName, $"{assemblyName}.dll")
                        ).FileVersion
            with _ -> ()

            MsgBoxDialog(
                PluginData.NppData.NppHandle,
                $"Current version: {version}\000",
                $"About {pluginName}",
                (uint) (MsgBox.ICONQUESTION ||| MsgBox.OK))
            |> ignore

        let sKey = ShortcutKey(TRUE, FALSE, TRUE, 116uy) // Ctrl + Shift + F5...harp!

        [ ("Say \"&Hello\"", Some(helloNpp), Some(sKey))
          ("-", None, None)
          ("&About", Some(displayInfo), None) ]
        |> List.iter ((fun (name, fn: (unit -> unit) option, key) ->
                match (fn, key) with
                | (Some (f), Some (k)) -> Utils.SetCommand(name, f, k)
                | (Some (f), _) -> Utils.SetCommand(name, f)
                | _ -> Utils.MakeSeparator())
            >> ignore)

    override __.OnBeNotified(notification: ScNotification) =
        if notification.Header.HwndFrom = PluginData.NppData.NppHandle then
            match EnumOfValue<uint, NppMsg>(notification.Header.Code) with
            | NppMsg.NPPN_READY ->
                // do some late-phase initialization
                ()
            | NppMsg.NPPN_TBMODIFICATION ->
                // create your toolbar icon(s)
                ()
            | NppMsg.NPPN_SHUTDOWN ->
                // clean up resources
                PluginData.PluginNamePtr <- 0n
                PluginData.FuncItems.Dispose()
            | _ -> ()
        else
            ()

    override __.OnMessageProc(msg: uint, wParam: unativeint, lParam: nativeint) : NativeBool =
        match msg with
        | Win32.WM_SIZE when (int) wParam = Win32.SIZE_MAXIMIZED ->
            let (height, width) = (lParam >>> 16, lParam &&& 0xFFFF)

            MsgBoxDialog(
                PluginData.NppData.NppHandle,
                $"New window size: {height}x{width}\000",
                $"{pluginName}",
                (uint) (MsgBox.ICONASTERISK ||| MsgBox.OK ||| MsgBox.TOPMOST))
            |> ignore
        | _ -> ()

        base.OnMessageProc(msg, wParam, lParam)

    /// <summary><see cref="Main"/> should be a singleton class</summary>
    private new() = { inherit DotNetPlugin() }
