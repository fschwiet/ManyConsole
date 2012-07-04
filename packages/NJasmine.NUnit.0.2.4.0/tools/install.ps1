
param($installPath, $toolsPath, $package, $project)

$nunitVersion = "2.6.0.12054";
$nunitRunnerPattern = "NUnit.Runners." + $nunitVersion.substring(0, $nunitVersion.lastIndexOf(".")) + ".*"

$nunitPaths = gci (join-path $installPath "..") $nunitRunnerPattern | % { $_.fullname }

foreach($nunitPath in $nunitPaths) {
    
    $targetPath = "$nunitPath\tools\addins";
    
    if (-not (test-path $targetPath)) {
        $null = mkdir $targetPath
    }
    
    cp "$installPath\lib\*" "$nunitPath\tools\addins"
}
