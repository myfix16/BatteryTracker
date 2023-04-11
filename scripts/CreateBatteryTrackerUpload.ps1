<#
    1. Copy msix packages and symbol files from the build output to the packages folder
    2. Create a msix bundle
    3. Create a zip file with the msix bundle and the symbol files
#>
param (
    [Parameter(Mandatory = $true)]
    [string]$version
)
# $version = "1.4.1.0"

$packagesFolder = Join-Path (Get-Item $PSScriptRoot).Parent "packages"
$msixsFolder = Join-Path $packagesFolder "msixs"
$symbolsFolder = Join-Path $packagesFolder "symbols"
$outputFormatString = Join-Path (Get-Item $PSScriptRoot).Parent "src/bin/{0}/Release/net7.0-windows10.0.22000.0/win10-{0}/AppPackages/BatteryTracker_{1}_Test"
$msixBundlePath = Join-Path $packagesFolder "BatteryTracker_${version}_x86_x64_arm64.msixbundle"
$msixUploadPath = Join-Path $packagesFolder "BatteryTracker_${version}_x86_x64_arm64_bundle.msixupload"
$archs = "x86", "x64", "arm64"

# Clean up the packages folder
if (Test-Path -Path $packagesFolder) { Remove-Item $packagesFolder -Recurse -Force }
New-Item $packagesFolder -Type Directory

# Copy msix packages and symbol files from the build output to the packages folder
foreach ($arch in $archs) {
    $outputPath = $outputFormatString -f $arch, $version
    $msixFileName = "BatteryTracker_${version}_${arch}.msix"
    $symbolFileName = "BatteryTracker_${version}_${arch}.msixsym"
    $msixSourcePath = Join-Path $outputPath $msixFileName
    $symbolSourcePath = Join-Path $outputPath $symbolFileName
    
    if (!(Test-Path -Path $msixsFolder)) { New-Item $msixsFolder -Type Directory }
    Copy-Item $msixSourcePath -Destination $msixsFolder -Force
    if (!(Test-Path -Path $symbolsFolder)) { New-Item $symbolsFolder -Type Directory }
    Copy-Item $symbolSourcePath -Destination $symbolsFolder -Force
}

# Create a msix bundle
makeappx.exe bundle -d $msixsFolder -p $msixBundlePath -v

# Create a zip file with the msix bundle and the symbol files
Get-ChildItem (Join-Path $symbolsFolder "*.msixsym") | Rename-Item -NewName { $_.Name -replace '.msixsym', '.appxsym' }
Compress-Archive -Path ($msixBundlePath, (Join-Path $symbolsFolder "*.appxsym")) -DestinationPath $msixUploadPath -Update -Confirm -CompressionLevel NoCompression
