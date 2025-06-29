# Timetable to JSON Converter PowerShell Script
# Usage: .\convert_timetable_v2.ps1 <timetable_text_file> [output_json_file]

param (
    [Parameter(Mandatory=$true)]
    [string]$TimetablePath,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath
)

# Set default output path if not provided
if (-not $OutputPath) {
    $filename = [System.IO.Path]::GetFileNameWithoutExtension($TimetablePath)
    $directory = [System.IO.Path]::GetDirectoryName($TimetablePath)
    $OutputPath = Join-Path -Path $directory -ChildPath "$filename.json"
}

Write-Host "Converting timetable from $TimetablePath to JSON..."
$pythonPath = "C:/Users/kevin/OneDrive - Department of Education/Documents/Visual Studio Projects/MySchool/venv/Scripts/python.exe"

# Run the timetable parser
if ($OutputPath) {
    & $pythonPath "modules/timetable_parser_v2.py" "$TimetablePath" "$OutputPath"
} else {
    & $pythonPath "modules/timetable_parser_v2.py" "$TimetablePath"
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "Conversion successful. JSON saved to: $OutputPath" -ForegroundColor Green
} else {
    Write-Host "Conversion failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

exit 0
