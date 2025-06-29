# PDF to JSON Timetable Converter PowerShell Script
# Usage: .\pdf_to_json_fixed.ps1 -PdfPath <pdf_file_path>

param (
    [Parameter(Mandatory=$true)]
    [string]$PdfPath
)

Write-Host "Converting PDF timetable from $PdfPath to JSON..."

# First extract text from PDF
Write-Host "Step 1: Extracting text from PDF..."
$pdfFilename = [System.IO.Path]::GetFileNameWithoutExtension($PdfPath)
$textPath = "c:\Users\kevin\OneDrive - Department of Education\Documents\Visual Studio Projects\MySchool\output\$pdfFilename.txt"

& "C:/Users/kevin/OneDrive - Department of Education/Documents/Visual Studio Projects/MySchool/venv/Scripts/python.exe" `
  "c:\Users\kevin\OneDrive - Department of Education\Documents\Visual Studio Projects\MySchool\modules\pdf_text_extractor.py" `
  "$PdfPath"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to extract text from PDF." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Then convert text to JSON
Write-Host "Step 2: Converting extracted text to JSON..."
& "C:/Users/kevin/OneDrive - Department of Education/Documents/Visual Studio Projects/MySchool/venv/Scripts/python.exe" `
  "c:\Users\kevin\OneDrive - Department of Education\Documents\Visual Studio Projects\MySchool\modules\timetable_to_json_fixed.py" `
  "$textPath"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to convert text to JSON." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Conversion complete! JSON file created at: output\$pdfFilename.json" -ForegroundColor Green
exit 0
