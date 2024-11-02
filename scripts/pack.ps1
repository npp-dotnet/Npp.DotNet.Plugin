#
# SPDX-FileCopyrightText: (c) 2024 Robert Di Pardo
# SPDX-License-Identifier: 0BSD
#
try {
    pushd $PSScriptRoot\..\lib
    [xml]$proj = Get-Content .\Npp.DotNet.Plugin.csproj
    $tfms = Select-Xml -Xml $proj -XPath '/Project/PropertyGroup/TargetFrameworks' | `
      Select-Object -ExpandProperty Node | `
      Select-object -ExpandProperty InnerText

  foreach($tfm in $tfms.Split(';'))
  {
      & dotnet build -c Release -f $tfm
  }
  & dotnet pack --no-build
} catch {
   $_.InvocationInfo.PositionMessage; $_.Exception.Message
} finally {
    popd
}
