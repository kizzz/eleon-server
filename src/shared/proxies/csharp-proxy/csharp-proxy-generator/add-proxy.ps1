param(
    [Parameter(Mandatory = $true)][string]$SwaggerUrl,
    [Parameter(Mandatory = $true)][string]$DestFolder,
    [Parameter(Mandatory = $true)][string]$ProxyName
)

Write-Host "Downloading swagger.json from $SwaggerUrl ..."


if (-not (Test-Path $DestFolder)) {
    New-Item -ItemType Directory -Path $DestFolder | Out-Null
}

# Download swagger.json
$swaggerPath = Join-Path $DestFolder "swagger.json"
Invoke-WebRequest -Uri $SwaggerUrl -OutFile $swaggerPath

if (-not (Test-Path $swaggerPath)) {
    Write-Host "Failed to download swagger.json"
    exit 1
}

# Ensure destination exists
if (-not (Test-Path $DestFolder)) {
    New-Item -ItemType Directory -Path $DestFolder | Out-Null
}

# Ensure src folder is clear
if (Test-Path "$DestFolder\src") {
    Remove-Item "$DestFolder\src" -Recurse -Force
}

Write-Host "Generating C# code from OpenAPI spec..."

npx @openapitools/openapi-generator-cli generate `
    -i $swaggerPath `
    -g csharp `
    -p "library=generichost,nullableReferenceTypes=false,targetFramework=net9.0,packageName=$ProxyName" `
    -o $DestFolder `
    -t ./csharp-templates

Read-Host "Press Enter to exit"
