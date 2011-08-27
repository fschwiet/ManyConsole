properties {
    $baseDirectory  = resolve-path .
    $buildDirectory = ($buildDirectory, "$baseDirectory\build") | select -first 1
    $version = "0.1.0"

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
			-copyright "Copyright © Frank Schwieterman 2011" `
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

        add-xmlnamespace "ns" "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"

        for-xml "//ns:package/ns:metadata" {
            set-xml -exactlyOnce "//ns:version" "$version.0"
            set-xml -exactlyOnce "//ns:owners" "fschwiet"
            set-xml -exactlyOnce "//ns:authors" "Frank Schwieterman"
            set-xml -exactlyOnce "//ns:description" $shortDescription

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
