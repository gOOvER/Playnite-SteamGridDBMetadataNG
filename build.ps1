param(
    [string]$Configuration = 'Release',
    [switch]$NoInstall
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$projectPath = Join-Path $root 'SGDBMetadata\SGDBMetadata.csproj'
$outDir = Join-Path $root "SGDBMetadata\bin\$Configuration\net462"

Write-Host "==> Building SteamGridDB Metadata Provider NG ($Configuration)..." -ForegroundColor Cyan
dotnet build $projectPath -c $Configuration -m:4
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

$pext = Get-ChildItem -Path (Join-Path $root 'SGDBMetadata\bin') -Filter "SteamGridDB_Playnite_Metadata*.pext" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($pext) {
    Write-Host "==> Package ready: $($pext.FullName)" -ForegroundColor Green
}

if (-not $NoInstall) {
    $extTarget = "$env:APPDATA\Playnite\Extensions\SteamGridDB_Playnite_Metadata"
    
    # Close Playnite if running so files are not locked
    Stop-Process -Name "Playnite.DesktopApp", "Playnite" -Force -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 500

    Write-Host "==> Installing to Playnite extensions directory: $extTarget..." -ForegroundColor Cyan
    if (!(Test-Path $extTarget)) {
        New-Item -ItemType Directory -Path $extTarget -Force | Out-Null
    }
    Copy-Item -Path "$outDir\*" -Destination $extTarget -Recurse -Force
    Write-Host "==> Extension successfully installed to $extTarget" -ForegroundColor Green
}
