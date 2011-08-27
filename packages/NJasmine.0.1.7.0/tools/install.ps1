
param($installPath, $toolsPath, $package, $project)

$nunitPaths = gci (join-path $installPath "..") "NUnit.2.5.10.*" | % { $_.fullname }
foreach($nunitPath in $nunitPaths) {
    $null = mkdir "$nunitPath\tools\addins"
    cp "$installPath\lib\*" "$nunitPath\tools\addins"
}