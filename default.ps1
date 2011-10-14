properties {
    $baseDirectory  = resolve-path .
    $buildDirectory = ($buildDirectory, "$baseDirectory\build") | select -first 1
    $version = "0.3.5"

    $shortDescription = "A library for writing console applications.  Extends NDesk.Options to support separate commands from one console application."
}

import-module .\tools\PSUpdateXML.psm1
. .\psake_ext.ps1

task default -depends Build,RunTests,BuildNuget

task Cleanup {
    if (test-path $buildDirectory) {
        rm $buildDirectory -recurse
    }
}

task GenerateAssemblyInfo {
	
	$projectFiles = ls -path $base_dir -include *.csproj -recurse

    $projectFiles | write-host
	foreach($projectFile in $projectFiles) {
		
		$projectDir = [System.IO.Path]::GetDirectoryName($projectFile)
		$projectName = [System.IO.Path]::GetFileName($projectDir)
		$asmInfo = [System.IO.Path]::Combine($projectDir, [System.IO.Path]::Combine("Properties", "AssemblyInfo.cs"))
				
		Generate-Assembly-Info `
			-file $asmInfo `
			-title "$projectName $version.0" `
			-description $shortDescription `
			-company "n/a" `
			-product "ManyConsole $version.0" `
			-version "$version.0" `
			-fileversion "$version.0" `
			-copyright "Copyright � Frank Schwieterman 2011" `
			-clsCompliant "false"
	}
}

task Build -depends Cleanup,GenerateAssemblyInfo {
    $v4_net_version = (ls "$env:windir\Microsoft.NET\Framework\v4.0*").Name
    $dearlySolution = "$baseDirectory\dearly.sln"

    exec { &"C:\Windows\Microsoft.NET\Framework\$v4_net_version\MSBuild.exe" ManyConsole.sln /T:"Clean,Build" /property:OutDir="$buildDirectory\" }    
}

task RunTests {
    exec { & "$baseDirectory\packages\NUnit.2.5.10.11092\tools\nunit-console.exe" "$buildDirectory\ManyConsole.Tests.dll" -xml:"$buildDirectory\TestResults.xml" }
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

        for-xml "//package/metadata" {
            set-xml -exactlyOnce "//version" "$version.0"
            set-xml -exactlyOnce "//owners" "fschwiet"
            set-xml -exactlyOnce "//authors" "Frank Schwieterman"
            set-xml -exactlyOnce "//description" $shortDescription

            set-xml -exactlyOnce "//licenseUrl" "https://github.com/fschwiet/ManyConsole/blob/master/LICENSE.txt"
            set-xml -exactlyOnce "//projectUrl" "https://github.com/fschwiet/ManyConsole/"
            remove-xml -exactlyOnce "//iconUrl"
            set-xml -exactlyOnce "//tags" "ndesk ndesk.options command-line console"
            set-xml -exactlyOnce "//releaseNotes" "Removed dependency on Json.NET.";

            set-xml -exactlyOnce "//dependencies" ""
            append-xml -exactlyOnce "//dependencies" "<dependency id=`"NDesk.Options`" version=`"0.2`" />"

            append-xml "." "<summary>Easily mix commands for a console application.</summary>"
        }
    }

    ..\..\tools\nuget pack "ManyConsole.nuspec"

    cd $old
}
