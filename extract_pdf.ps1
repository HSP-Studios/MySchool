# PDF Text Extractor PowerShell Wrapper Script
# Usage: .\extract_pdf.ps1 <pdf_file_path>

param (
    [Parameter(Mandatory=$true)]
    [string]$PdfPath
)

Write-Host "Extracting text from $PdfPath..."
& "C:/Users/kevin/OneDrive - Department of Education/Documents/Visual Studio Projects/MySchool/venv/Scripts/python.exe" `
  "c:\Users\kevin\OneDrive - Department of Education\Documents\Visual Studio Projects\MySchool\modules\pdf_text_extractor.py" `
  "$PdfPath"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to extract text from PDF." -ForegroundColor Red
    exit $LASTEXITCODE
}

exit 0
