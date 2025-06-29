# Timetable Text to JSON Converter PowerShell Wrapper Script
# Usage: .\convert_timetable_fixed.ps1 <text_file_path>

param (
    [Parameter(Mandatory=$true)]
    [string]$TextPath
)

Write-Host "Converting timetable text from $TextPath to JSON..."
& "C:/Users/kevin/OneDrive - Department of Education/Documents/Visual Studio Projects/MySchool/venv/Scripts/python.exe" `
  "c:\Users\kevin\OneDrive - Department of Education\Documents\Visual Studio Projects\MySchool\modules\timetable_to_json_fixed.py" `
  "$TextPath"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to convert timetable text to JSON." -ForegroundColor Red
    exit $LASTEXITCODE
}

exit 0
