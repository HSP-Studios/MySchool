# PDF to JSON All-in-One Script
# This script extracts text from a PDF and converts it directly to JSON
# Usage: .\pdf_to_json.ps1 -PdfPath <pdf_file_path>

param (
    [Parameter(Mandatory=$true)]
    [string]$PdfPath
)

$pythonExe = "C:/Users/kevin/OneDrive - Department of Education/Documents/Visual Studio Projects/MySchool/venv/Scripts/python.exe"
$extractorScript = "c:\Users\kevin\OneDrive - Department of Education\Documents\Visual Studio Projects\MySchool\modules\pdf_text_extractor.py"
$converterScript = "c:\Users\kevin\OneDrive - Department of Education\Documents\Visual Studio Projects\MySchool\modules\timetable_to_json.py"

# Check if PDF file exists
if (-not (Test-Path $PdfPath)) {
    Write-Host "Error: PDF file not found at $PdfPath" -ForegroundColor Red
    exit 1
}

# Get the base name of the PDF file
$pdfBaseName = [System.IO.Path]::GetFileNameWithoutExtension($PdfPath)
$outputDir = "c:\Users\kevin\OneDrive - Department of Education\Documents\Visual Studio Projects\MySchool\output"
$textFilePath = "$outputDir\$pdfBaseName.txt"

# Step 1: Extract text from PDF
Write-Host "Step 1: Extracting text from $PdfPath..." -ForegroundColor Cyan
& $pythonExe $extractorScript $PdfPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to extract text from PDF." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Check if text file was created
if (-not (Test-Path $textFilePath)) {
    Write-Host "Error: Text file was not created at $textFilePath" -ForegroundColor Red
    exit 1
}

# Step 2: Convert text to JSON
Write-Host "Step 2: Converting text to JSON..." -ForegroundColor Cyan
& $pythonExe $converterScript $textFilePath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to convert text to JSON." -ForegroundColor Red
    exit $LASTEXITCODE
}

$jsonFilePath = "$outputDir\$pdfBaseName.json"
if (Test-Path $jsonFilePath) {
    Write-Host "Success: JSON file created at $jsonFilePath" -ForegroundColor Green
} else {
    Write-Host "Warning: JSON file was not found at the expected location $jsonFilePath" -ForegroundColor Yellow
}

exit 0
