# PDF to JSON Converter PowerShell Script (V2)
# Usage: .\pdf_to_json_v2.ps1 -PdfPath <pdf_file_path> [-OutputPath <output_json_path>]

param (
    [Parameter(Mandatory=$true)]
    [string]$PdfPath,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath
)

# Set default output path if not provided
if (-not $OutputPath) {
    $filename = [System.IO.Path]::GetFileNameWithoutExtension($PdfPath)
    $directory = [System.IO.Path]::GetDirectoryName($PdfPath)
    $OutputPath = Join-Path -Path $directory -ChildPath "$filename.json"
}

$pythonPath = "C:/Users/kevin/OneDrive - Department of Education/Documents/Visual Studio Projects/MySchool/venv/Scripts/python.exe"
$textOutputPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($OutputPath), [System.IO.Path]::GetFileNameWithoutExtension($PdfPath) + ".txt")

# Step 1: Extract text from PDF
Write-Host "Step 1: Extracting text from PDF..."
& $pythonPath "modules/pdf_text_extractor.py" "$PdfPath"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to extract text from PDF." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Step 2: Convert text to JSON using the new parser
Write-Host "Step 2: Converting extracted text to JSON..."
& $pythonPath "modules/timetable_parser_v2.py" "$textOutputPath" "$OutputPath"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to convert text to JSON." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Success! PDF timetable has been converted to JSON." -ForegroundColor Green
Write-Host "Output JSON file: $OutputPath"

exit 0
