properties {
    $baseDirectory  = resolve-path .
    $buildDirectory = ($buildDirectory, "$baseDirectory\build") | select -first 1
    $version = "3.0.0"
}

import-module .\tools\PSUpdateXML.psm1

task default -depends Build,BuildNuget

task Cleanup {
    if (test-path $buildDirectory) {
        rm $buildDirectory -recurse
    }
}

task Build -depends Cleanup {
    $v4_net_version = (ls "$env:windir\Microsoft.NET\Framework\v4.0*").Name
    $dearlySolution = "$baseDirectory\dearly.sln"

    exec { &"C:\Windows\Microsoft.NET\Framework\$v4_net_version\MSBuild.exe" ManyConsole.sln /T:"Clean,Build" /property:OutDir="$buildDirectory\" }    
}

task BuildNuget -depends Build {

    $nugetTarget = "$buildDirectory\nuget"

    $null = mkdir "$nugetTarget\lib\"
    $null = mkdir "$nugetTarget\tools\"

    cp "$buildDirectory\ManyConsole.dll" "$nugetTarget\lib\"
    cp "$buildDirectory\ManyConsole.pdb" "$nugetTarget\lib\"

    $old = pwd
    cd $nugetTarget

    ..\..\tools\nuget.exe spec -a ".\lib\ManyConsole.dll"

    update-xml "ManyConsole.nuspec" {

        add-xmlnamespace "ns" "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"

        for-xml "//ns:package/ns:metadata" {
            set-xml -exactlyOnce "//ns:version" "$version.0"
            set-xml -exactlyOnce "//ns:owners" "fschwiet"
            set-xml -exactlyOnce "//ns:authors" "Frank Schwieterman"
            set-xml -exactlyOnce "//ns:description" "A library for writing console applicatoins.  Extends NDesk.Options to support separate commands from one console application."

            set-xml -exactlyOnce "//ns:licenseUrl" "https://github.com/fschwiet/ManyConsole/blob/master/LICENSE.txt"
            set-xml -exactlyOnce "//ns:projectUrl" "https://github.com/fschwiet/ManyConsole/"
            remove-xml -exactlyOnce "//ns:iconUrl"
            set-xml -exactlyOnce "//ns:tags" "Console"

            set-xml -exactlyOnce "//ns:dependencies" ""
            append-xml -exactlyOnce "//ns:dependencies" "<dependency id=`"Newtonsoft.Json`" version=`"4.0`" />"
            append-xml -exactlyOnce "//ns:dependencies" "<dependency id=`"NDesk.Options`" version=`"0.2`" />"

            append-xml "." "<summary>Easily mix commands for a console application.</summary>"
        }
    }

    ..\..\tools\nuget pack "ManyConsole.nuspec"

    cd $old
}
