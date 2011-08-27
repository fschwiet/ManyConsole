properties {
    $baseDirectory  = resolve-path .
    $buildDirectory = ($buildDirectory, "$baseDirectory\build") | select -first 1
}

task default -depends Build

task Cleanup {
    if (test-path $buildDirectory) {
        rm $buildDirectory -recurse
    }
}

task Build {
    $v4_net_version = (ls "$env:windir\Microsoft.NET\Framework\v4.0*").Name
    $dearlySolution = "$baseDirectory\dearly.sln"

    exec { &"C:\Windows\Microsoft.NET\Framework\$v4_net_version\MSBuild.exe" ManyConsole.sln /T:"Clean,Build" /property:OutDir="$buildDirectory\"` }    
}