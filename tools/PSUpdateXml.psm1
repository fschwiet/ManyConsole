


$license = "`
Copyright (c) 2010 Frank Schwieterman`
`
Permission is hereby granted, free of charge, to any person obtaining`
a copy of this software and associated documentation files (the`
`"Software`"), to deal in the Software without restriction, including`
without limitation the rights to use, copy, modify, merge, publish,`
distribute, sublicense, and/or sell copies of the Software, and to`
permit persons to whom the Software is furnished to do so, subject to`
the following conditions:`
`
The above copyright notice and this permission notice shall be`
included in all copies or substantial portions of the Software.`
`
THE SOFTWARE IS PROVIDED `"AS IS`", WITHOUT WARRANTY OF ANY KIND,`
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF`
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND`
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE`
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION`
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION`
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.`
`
`
orignal source/documentation: https://github.com/fschwiet/psupdatexml
";



$currentNamespaceManager = $null;
$currentNode = $null;


function update-xml([System.IO.FileInfo]$xmlFile, 
    [ScriptBlock]$action) {

    $doc = New-Object System.Xml.XmlDocument
    $currentNamespaceManager = New-Object System.Xml.XmlNamespaceManager $doc.NameTable
    $currentNode = $doc

    $xmlFile = (resolve-path $xmlFile).path;

    $doc.Load($xmlFile)

    & $action
     
    $doc.Save($xmlFile)
}

function add-xmlnamespace([string] $name, [string] $value) {
    $currentNamespaceManager.AddNamespace( $name, $value);
}

function check-quantifier-against-nodes($nodes, $exactlyonce,  $atleastonce,  $atmostonce) {
    
    if ($exactlyonce) {
        Assert ($nodes.length -eq 1) "Expected to find one match, actually found $($nodes.length) matches for xpath expression `"$xpath`"."
    }
    
    if ($atleastonce) {
        Assert ($nodes.length -ge 1) "Expected to find at least one match, actually found $($nodes.length) matches for xpath expression `"$xpath`"."
    }
    if ($atmostonce) {
        Assert ($nodes.length -le 1) "Expected to find at most one match, actually found $($nodes.length) matches for xpath expression `"$xpath`"."
    }
}


function get-xml([string] $xpath, 
    [switch]$exactlyonce = $false, 
    [switch]$atleastonce = $false, 
    [switch]$atmostonce = $false) {
    
    $nodes = @($currentNode.SelectNodes($xpath, $currentNamespaceManager))
     
    check-quantifier-against-nodes $nodes $exactlyonce $atleastonce $atmostonce

    foreach ($node in $nodes) {
        if ($node.NodeType -eq "Element") {
            $node.InnerXml
        }
        else {
            $node.Value
        }
    }
}

function set-xml(
    [string] $xpath, 
    [string] $value, 
    [switch]$exactlyonce = $false, 
    [switch]$atleastonce = $false, 
    [switch]$atmostonce = $false) {

    $nodes = @($currentNode.SelectNodes($xpath, $currentNamespaceManager))
    
    check-quantifier-against-nodes $nodes $exactlyonce $atleastonce $atmostonce
 
    foreach ($node in $nodes) {
        if ($node.NodeType -eq "Element") {
            $node.InnerXml = $value
        }
        else {
        
            if ($value) {
                $node.Value = $value
            } else {
                
                $nav = $node.CreateNavigator();
                $nav.DeleteSelf();
            }
        }
    }
}

function set-attribute(
    [string] $name,
    [string] $value) {
    
    if ($value) {
        $currentNode.SetAttribute($name, $value);
    } else {
        $currentNode.RemoveAttribute($name);
    }
}


function remove-xml([string] $xpath, 
    [switch]$exactlyonce = $false, 
    [switch]$atleastonce = $false, 
    [switch]$atmostonce = $false) {

    $nodes = @($currentNode.SelectNodes($xpath, $currentNamespaceManager))
     
    check-quantifier-against-nodes $nodes $exactlyonce $atleastonce $atmostonce

    foreach($node in $nodes) {
        $nav = $node.CreateNavigator();
        $nav.DeleteSelf();
    }
}

function append-xml([string] $xpath, 
    [string] $value, 
    [switch]$exactlyonce = $false, 
    [switch]$atleastonce = $false, 
    [switch]$atmostonce = $false) {

    $nodes = @($currentNode.SelectNodes($xpath, $currentNamespaceManager))
     
    check-quantifier-against-nodes $nodes $exactlyonce $atleastonce $atmostonce

    foreach($node in $nodes) {
        $nav = $node.CreateNavigator();
        $nav.AppendChild($value);
    }
}


function for-xml([string] $xpath, 
    [ScriptBlock] $action, 
    [switch]$exactlyonce = $false, 
    [switch]$atleastonce = $false, 
    [switch]$atmostonce = $false) {

    $originalNode = $currentNode
    
    try {
        $nodes = @($currentNode.SelectNodes($xpath, $currentNamespaceManager))

        check-quantifier-against-nodes $nodes $exactlyonce $atleastonce $atmostonce

        foreach($node in $nodes) {
            $currentNode = $node;
            & $action;
        }

    } finally {
        $currentNode = $originalNode
    }
}


export-modulemember -function update-xml,add-xmlnamespace,get-xml,set-xml,set-attribute,remove-xml,append-xml,for-xml


