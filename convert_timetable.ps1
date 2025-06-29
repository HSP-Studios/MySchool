# Timetable to JSON Converter PowerShell Wrapper Script
# Usage: .\convert_timetable.ps1 <text_file_path> [output_json_path]

param (
    [Parameter(Mandatory=$true)]
    [string]$TextFilePath,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputJsonPath
)

Write-Host "Converting timetable text from $TextFilePath to JSON..."

$command = "& ""C:/Users/kevin/OneDrive - Department of Education/Documents/Visual Studio Projects/MySchool/venv/Scripts/python.exe"" ""c:\Users\kevin\OneDrive - Department of Education\Documents\Visual Studio Projects\MySchool\modules\timetable_to_json.py"" ""$TextFilePath"""

if ($OutputJsonPath) {
    $command += " ""$OutputJsonPath"""
    Write-Host "Output will be saved to $OutputJsonPath"
}

Invoke-Expression $command

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to convert timetable to JSON." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Conversion completed successfully." -ForegroundColor Green
exit 0
