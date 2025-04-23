'
' SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
' SPDX-License-Identifier: Apache-2.0
'
Option Explicit On
Option Strict On

Imports System.Diagnostics.FileVersionInfo
Imports System.Runtime.InteropServices
Imports Npp.DotNet.Plugin.Win32

''' <summary>
''' Implements <see cref="IDotNetPlugin"/>.
''' </summary>
Public Class Main
    Implements IDotNetPlugin

#Region "1. Initialize"
    ''' <summary>
    ''' Use this to initialize any data your plugin needs when starting up.
    ''' At the very least, assign a unique name to the static <see cref="PluginData.PluginNamePtr"/> property.
    ''' Otherwise <see cref="PluginData.DefaultPluginName"/> will be used.
    ''' </summary>
    ''' <remarks>
    ''' This must be <see langword="Shared"/> so that initialization is automatic when the <see cref="Instance"/>
    ''' member is accessed by the unmanaged methods of the <see cref="Bridge"/> class.
    ''' </remarks>
    Shared Sub New()
        Instance = New Main()
        PluginData.PluginNamePtr = Marshal.StringToHGlobalUni(PluginName)
    End Sub
#End Region

#Region "2. Implement the plugin interface"
    Public Sub OnSetInfo() Implements IDotNetPlugin.OnSetInfo
        Dim sKey As New ShortcutKey([TRUE], [FALSE], [TRUE], 121) ' Ctrl + Shift + F10
        Utils.SetCommand("Say ""&Hello""", AddressOf HelloNpp, sKey)
        Utils.MakeSeparator()
        Utils.SetCommand("&About", AddressOf DisplayInfo)
    End Sub

    Public Sub OnBeNotified(Notification As ScNotification) Implements IDotNetPlugin.OnBeNotified
        If Notification.Header.HwndFrom = PluginData.NppData.NppHandle Then
            Dim code As UInteger = Notification.Header.Code
            Select Case CType(code, NppMsg)
                Case NppMsg.NPPN_READY
                    ' do some late-phase initialization
                Case NppMsg.NPPN_TBMODIFICATION
                    ' create your toolbar icon(s)
                Case NppMsg.NPPN_SHUTDOWN
                    ' clean up resources
                    PluginData.PluginNamePtr = IntPtr.Zero
                    PluginData.FuncItems.Dispose()
            End Select
        End If
    End Sub

    Public Function OnMessageProc(Msg As UInteger, WParam As UIntPtr, LParam As IntPtr) As NativeBool _
      Implements IDotNetPlugin.OnMessageProc
        Select Case Msg
            Case WM_SIZE
                If CInt(WParam) = SIZE_MAXIMIZED Then
                    Dim size = (height := CLng(LParam) >> 16, width := CLng(LParam) And &HFFFF)
                    MsgBoxDialog(PluginData.NppData.NppHandle,
                                 $"New window size: {size.height}x{size.width}" & ChrW(0),
                                 $"{PluginName}",
                                 CUInt(MsgBox.ICONASTERISK Or MsgBox.OK Or MsgBox.TOPMOST))
                End If
        End Select
        OnMessageProc = [TRUE]
    End Function
#End Region

#Region "3. Implement plugin commands"
    ''' <summary>
    ''' Creates a new buffer and inserts text into it.
    ''' </summary>
    Shared Sub HelloNpp()
        PluginData.Notepad.FileNew()
        PluginData.Editor.SetText("Hello, Notepad++ ... from VB.NET!")
    End Sub

    ''' <summary>
    ''' Shows the plugin's version number in a system dialog.
    ''' </summary>
    Shared Sub DisplayInfo()
        Dim version As String = "1.0.0.0"
        Try
            Dim assemblyName As String = GetType(Main).Namespace
            version =
                GetVersionInfo(
                    System.IO.Path.Combine(
                        PluginData.Notepad.GetPluginsHomePath(), assemblyName, $"{assemblyName}.dll")
                    ).FileVersion
        Catch
            Exit Try
        End Try

        MsgBoxDialog(PluginData.NppData.NppHandle,
                    $"Current version: {version}" & ChrW(0),
                    $"About {PluginName}",
                    CUInt(MsgBox.ICONQUESTION Or MsgBox.OK))
    End Sub
#End Region

    Public Shared ReadOnly Instance As IDotNetPlugin

    ''' <summary><see cref="Main"/> should be a singleton class</summary>
    Private Sub New()
    End Sub

    Private Shared ReadOnly PluginName As String = "VB.NET Demo Plugin" & ChrW(0)
End Class
