@echo off

set URL=https://localhost:5050/swagger/v1/swagger.json

echo Downloading swagger.json from %URL%...

REM check if folder temp exist and remove all exists files. trim last \ .
if not exist ..\eleonsoft-csharp-proxy (
	mkdir ..\eleonsoft-csharp-proxy
) 
REM else (
	REM remove all from dir
REM )

curl -o ..\eleonsoft-csharp-proxy\swagger.json %URL%
if ERRORLEVEL 1 (
    echo "Failed to download swagger.json"
    exit /b 1
)

echo Generating C# code from OpenAPI spec...

@REM openapi-generator-cli generate -g csharp -p library=generichost,nullableReferenceTypes=false,targetFramework=net8.0 -i %OUTPUT_DIR%\temp\swagger.json -o %OUTPUT_DIR%\sdk -t ./csharp-templates

npx @openapitools/openapi-generator-cli generate -i ../eleonsoft-csharp-proxy/swagger.json -g csharp -p library=generichost,nullableReferenceTypes=false,targetFramework=net9.0,packageName=EleonsoftProxy -o ../eleonsoft-csharp-proxy -t ./csharp-templates

pause