
Npp.DotNet.Plugin
-----------------

[![Current Version]][nuget-org]

A .NET (Core) library for developing Notepad++ plugins


### Quick start

Install the [.NET SDK](https://dotnet.microsoft.com/download), then install the [`Npp.DotNet.Templates`] package:

    dotnet new install Npp.DotNet.Templates

Create a new plugin project by running [`dotnet new`] `<template> --name <your_project_name>`

Open `<your_project_name>/README.md` for build instructions.


### Building a plugin from scratch

First, create a new library project, e.g.,

    dotnet new classlib --name YourPluginProject

Make sure the project's `<TargetFramework>` and `<RuntimeIdentifiers>` properties specify the Windows platform.

Also make sure to set the `<PublishAot>` and `<AllowUnsafeBlocks>` properties, e.g.,

```diff
  <PropertyGroup>
-   <TargetFramework>net10.0</TargetFramework>
+   <TargetFramework>net10.0-windows</TargetFramework>
+   <RuntimeIdentifiers>win-arm64;win-x64</RuntimeIdentifiers>
+   <PublishAot>true</PublishAot>
+   <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
```

Add the `Npp.DotNet.Plugin` package to the project:

    cd YourPluginProject
    dotnet add package Npp.DotNet.Plugin --prerelease

Define your plugin's main class, e.g.,

<details>
<summary>Class1.cs</summary>

```csharp
namespace YourPluginProject;

using Npp.DotNet.Plugin;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class Class1 : IDotNetPlugin
{
    #region "Implement the plugin interface"
    /// <summary>
    /// This method runs when Notepad++ calls the 'setInfo' API function.
    /// You can assume the application window handle is valid here.
    /// </summary>
    public void OnSetInfo()
    {
        // TODO: provide setup code, i.e., assign plugin commands to shortcut keys, load configuration data, etc.
        // For example:
        Utils.SetCommand(
            "About",
            () => Win32.MsgBoxDialog(
                PluginData.NppData.NppHandle,
                $"Information about {PluginName}.",
                $"About {PluginName}",
                (uint)(Win32.MsgBox.ICONASTERISK | Win32.MsgBox.OK)),
            new ShortcutKey(ctrl: Win32.TRUE, alt: Win32.FALSE, shift: Win32.TRUE, ch: 123 /* F12 */));
    }

    /// <summary>
    /// This method runs when Notepad++ calls the 'beNotified' API function.
    /// </summary>
    public void OnBeNotified(ScNotification notification)
    {
        // TODO: provide callbacks for editor events and notifications.
        // For example:
        if (notification.Header.HwndFrom == PluginData.NppData.NppHandle)
        {
            uint code = notification.Header.Code;
            switch ((NppMsg)code)
            {
                case NppMsg.NPPN_TBMODIFICATION:
                    PluginData.FuncItems.RefreshItems();
                    // TODO: register toolbar icon(s)
                    break;
                case NppMsg.NPPN_SHUTDOWN:
                    // clean up resources
                    PluginData.PluginNamePtr = IntPtr.Zero;
                    PluginData.FuncItems.Dispose();
                    break;
            }
        }
    }

    /// <summary>
    /// This method runs when Notepad++ calls the 'messageProc' API function.
    /// </summary>
    public NativeBool OnMessageProc(uint msg, UIntPtr wParam, IntPtr lParam)
    {
        // TODO: provide callbacks for Win32 window messages.
        return Win32.TRUE;
    }
    #endregion

    #region "Initialize your plugin's properties"
    /// <summary>
    /// Object reference to the main class -- must be initialized statically!
    /// </summary>
    static readonly IDotNetPlugin Instance;

    /// <summary>
    /// The unique name of the plugin -- appears in the 'Plugins' drop-down menu
    /// </summary>
    static readonly string PluginName = "Your .NET SDK Plugin";

    /// <summary>
    /// The main constructor must be static to ensure data is initialized *before*
    /// the Notepad++ application calls any unmanaged methods.
    /// At the very least, assign a unique name to 'Npp.DotNet.Plugin.PluginData.PluginNamePtr',
    /// otherwise the default name -- "Npp.DotNet.Plugin" -- will be used.
    /// </summary>
    static Class1()
    {
        Instance = new Class1();
        PluginData.PluginNamePtr = Marshal.StringToHGlobalUni(PluginName);
    }
    #endregion

    #region "==================== COPY & PASTE *ONLY* ========================"
    [UnmanagedCallersOnly(EntryPoint = "setInfo", CallConvs = [typeof(CallConvCdecl)])]
    internal unsafe static void SetInfo(NppData* notepadPlusData)
    {
        PluginData.NppData = *notepadPlusData;
        Instance.OnSetInfo();
    }

    [UnmanagedCallersOnly(EntryPoint = "beNotified", CallConvs = [typeof(CallConvCdecl)])]
    internal unsafe static void BeNotified(ScNotification* notification)
    {
        Instance.OnBeNotified(*notification);
    }

    [UnmanagedCallersOnly(EntryPoint = "messageProc", CallConvs = [typeof(CallConvCdecl)])]
    internal static NativeBool MessageProc(uint msg, UIntPtr wParam, IntPtr lParam)
    {
        return Instance.OnMessageProc(msg, wParam, lParam);
    }

    [UnmanagedCallersOnly(EntryPoint = "getFuncsArray", CallConvs = [typeof(CallConvCdecl)])]
    internal static IntPtr GetFuncsArray(IntPtr nbF) => IDotNetPlugin.OnGetFuncsArray(nbF);

    [UnmanagedCallersOnly(EntryPoint = "getName", CallConvs = [typeof(CallConvCdecl)])]
    internal static IntPtr GetName() => IDotNetPlugin.OnGetName();

    [UnmanagedCallersOnly(EntryPoint = "isUnicode", CallConvs = [typeof(CallConvCdecl)])]
    internal static NativeBool IsUnicode() => IDotNetPlugin.OnIsUnicode();
    #endregion
}
```

</details>

Build the plugin:

    dotnet publish -c Release -r win-x64


See [these examples](https://github.com/npp-dotnet/Npp.DotNet.Plugin/tree/main/examples) to learn more.


### Projects using `Npp.DotNet.Plugin`

* [Redis](https://github.com/bakingam1983/Notepad.plus.plus-RedisPlugin): a plugin for viewing Redis keys and values
* [WebEdit](https://github.com/Krazal/WebEdit): a plugin for quickly inserting configurable HTML tag pairs


### License

```
    (c) 2016-2026 Kasper B. Graversen, Mark Johnston Olson, Robert Di Pardo

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.

```

See the [COPYING] and [NOTICE] files for complete details regarding source code attribution and permitted usage.


[COPYING]: https://raw.githubusercontent.com/npp-dotnet/npp.dotnet.plugin/main/COPYING
[NOTICE]: https://raw.githubusercontent.com/npp-dotnet/npp.dotnet.plugin/main/NOTICE.txt
[Current Version]: https://img.shields.io/nuget/vpre/Npp.DotNet.Plugin?color=blueviolet&logo=nuget
[nuget-org]: https://www.nuget.org/packages/Npp.DotNet.Plugin
[`Npp.DotNet.Templates`]: https://www.nuget.org/packages/Npp.DotNet.Templates
[`dotnet new`]: https://learn.microsoft.com/dotnet/core/tools/dotnet-new
