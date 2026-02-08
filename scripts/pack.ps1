#
# SPDX-FileCopyrightText: (c) 2024 Robert Di Pardo
# SPDX-License-Identifier: 0BSD
#
try {
    $HEAD=$(git describe --always)
    $PREV_REF=$(git describe --always "${HEAD}^")
    $PREV=$(git describe --always --abbrev=0 "${PREV_REF}")
    $CHANGELOG=$(& git log --pretty='format:%x2d%x20%s' --no-merges -P -i --invert-grep --grep='(?|action|dependabot|release)' "$PREV..$HEAD")
    $env:PackageReleaseNotes=$(@($CHANGELOG) -join "`r`n")
    pushd $PSScriptRoot\..\lib
    [xml]$proj = Get-Content .\Npp.DotNet.csproj
    $tfms = Select-Xml -Xml $proj -XPath '/Project/PropertyGroup/TargetFrameworks' | `
      Select-Object -ExpandProperty Node | `
      Select-object -ExpandProperty InnerText

  foreach($tfm in $tfms.Split(';'))
  {
      & dotnet build -c Release -f $tfm
  }
  & dotnet pack --no-build -o .\bin\Release
} catch {
   $_.InvocationInfo.PositionMessage; $_.Exception.Message
} finally {
    popd
}
