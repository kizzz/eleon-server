 param (
    [Parameter(Mandatory=$true)][string]$Path,
    [Parameter(Mandatory=$true)][string]$OldText,
    [Parameter(Mandatory=$true)][string]$NewText
 )

Get-ChildItem -Recurse -File -Path $Path | ForEach {  (Get-Content $_.PSPath | ForEach {$_ -creplace $OldText, $NewText}) | Set-Content $_.PSPath }
