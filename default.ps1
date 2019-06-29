properties {
    $baseDirectory  = resolve-path .
    $buildDirectory = ($buildDirectory, "$baseDirectory\build") | select -first 1
    $version = "2.0.0-beta"

    $shortDescription = "A library for writing console applications.  Extends Mono.Options to support separate commands from one console application."
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

    $trimmedVersion = $version;

    if ($trimmedVersion.indexOf("-") -gt -1) {
        $trimmedVersion = $trimmedVersion.Substring(0, $trimmedVersion.indexOf("-"));
    }
	
	$projectFiles = ls -path $base_dir -include *.csproj -recurse

    $projectFiles | write-host
	foreach($projectFile in $projectFiles) {
		
        $projectDir = [System.IO.Path]::GetDirectoryName($projectFile)
        
		$projectName = [System.IO.Path]::GetFileName($projectDir)
		$asmInfo = [System.IO.Path]::Combine($projectDir, [System.IO.Path]::Combine("Properties", "AssemblyInfo.cs"))
				
		Generate-Assembly-Info `
			-file $asmInfo `
			-title "$projectName $version" `
			-description $shortDescription `
			-company "n/a" `
			-product "ManyConsole $version" `
			-version "$trimmedVersion" `
			-fileversion "$trimmedVersion" `
			-copyright "Copyright © Frank Schwieterman 2019" `
			-clsCompliant "false"
	}
}

task Build -depends Cleanup,GenerateAssemblyInfo {
    exec { & dotnet build ManyConsole.sln -o "$buildDirectory\" -c Release }    
}

task RunTests {
    exec { & dotnet test --logger trx }
}

task BuildNuget -depends Build {

    $nugetTarget = "$buildDirectory\nuget"

    $null = mkdir "$nugetTarget\"
    cp .\ManyConsole.nuspec "$nugetTarget\"

    $old = pwd
    cd $nugetTarget

    update-xml "ManyConsole.nuspec" {

        for-xml "//package/metadata" {
            set-xml -exactlyOnce "//version" "$version"
            set-xml -exactlyOnce "//description" $shortDescription
        }
    }

    ..\..\tools\nuget pack "ManyConsole.nuspec"

    cd $old
}
