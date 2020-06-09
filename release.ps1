$array = @("DragAndDrop.AISyoujyo", "DragAndDrop.HoneySelect2", "DragAndDrop.Koikatu", "DragAndDrop.PlayHome")

if ($PSScriptRoot -match '.+?\\bin\\?') {
    $dir = $PSScriptRoot + "\"
}
else {
    $dir = $PSScriptRoot + "\bin\"
}

$out = $dir + "BepInEx\plugins\" 
New-Item -ItemType Directory -Force -Path $out
New-Item -ItemType Directory -Force -Path ($dir + "out\")

# Fix using wrong slashes in zip files
Add-Type -AssemblyName System.Text.Encoding
Add-Type -AssemblyName System.IO.Compression.FileSystem
class FixedEncoder : System.Text.UTF8Encoding {
    FixedEncoder() : base($true) { }

    [byte[]] GetBytes([string] $s)
    {
        $s = $s.Replace("\\", "/");
        return ([System.Text.UTF8Encoding]$this).GetBytes($s);
    }
}

function CreateZip ($element)
{
    Remove-Item -Force -Path ($out + "*")
    New-Item -ItemType Directory -Force -Path $out

    Copy-Item -Path ($dir + $element + ".dll") -Destination $out
    Copy-Item -Path ($dir + $element + ".xml") -Destination $out -ErrorAction Ignore

    $ver = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($dir + $element + ".dll").FileVersion.ToString()

    $zipName = $dir + "out\" + $element + "_" + $ver + ".zip";
    Remove-Item -Force -Path $zipName -ErrorAction Ignore
    [System.IO.Compression.ZipFile]::CreateFromDirectory($dir + "BepInEx", $zipName, [System.IO.Compression.CompressionLevel]::Optimal, $true, [FixedEncoder]::new())
}

foreach ($element in $array) 
{
    try
    {
        CreateZip ($element)
    }
    catch 
    {
        # retry
        CreateZip ($element)
    }
}

Remove-Item -Force -Path ($out + "*")
Remove-Item -Force -Path ($dir + "BepInEx") -Recurse

