
#  brazenly borrowed from https://github.com/ayende/ravendb


function Get-File-Exists-On-Path
{
	param(
		[string]$file
	)
	$results = ($Env:Path).Split(";") | Get-ChildItem -filter $file -erroraction silentlycontinue
	$found = ($results -ne $null)
	return $found
}

function Get-Git-Commit {
	if ((Get-File-Exists-On-Path "git.exe")){
		$gitLog = git log --oneline -1
		return $gitLog.Split(' ')[0]
	}
	else {
		return "0000000"
	}
}


function Generate-Assembly-Info {
param(
	[string]$clsCompliant = "true",
	[string]$title, 
	[string]$description, 
	[string]$company, 
	[string]$product, 
	[string]$copyright, 
	[string]$version,
	[string]$fileVersion,
	[string]$file = $(throw "file is a required parameter.")
)
  
  if($env:buildlabel -eq 13 -and (Test-Path $file))
	{
      return
	}
   
   $commit = Get-Git-Commit
 
  
  $asmInfo = "using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !SILVERLIGHT
[assembly: SuppressIldasmAttribute()]
#endif
[assembly: CLSCompliantAttribute($clsCompliant )]
[assembly: ComVisibleAttribute(false)]
[assembly: AssemblyTitleAttribute(""$title"")]
[assembly: AssemblyDescriptionAttribute(""$description"")]
[assembly: AssemblyCompanyAttribute(""$company"")]
[assembly: AssemblyProductAttribute(""$product"")]
[assembly: AssemblyCopyrightAttribute(""$copyright"")]
[assembly: AssemblyVersionAttribute(""$version"")]
[assembly: AssemblyInformationalVersionAttribute(""$version / $commit"")]
[assembly: AssemblyFileVersionAttribute(""$fileVersion"")]
[assembly: AssemblyDelaySignAttribute(false)]
"

	$dir = [System.IO.Path]::GetDirectoryName($file)
	if ([System.IO.Directory]::Exists($dir) -eq $false)
	{
		Write-Host "Creating directory $dir"
		[System.IO.Directory]::CreateDirectory($dir)
	}
	Write-Host "Generating assembly info file: $file"
	$asmInfo | set-content $file -encoding utf8
}

