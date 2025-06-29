@echo off
rem Timetable Text to JSON Converter Wrapper Script
rem Usage: convert_timetable_fixed.bat <text_file_path>

if "%1"=="" (
    echo Error: No text file provided.
    echo Usage: convert_timetable_fixed.bat ^<text_file_path^>
    exit /b 1
)

set TEXT_PATH=%1

echo Converting timetable text from %TEXT_PATH% to JSON...
"C:/Users/kevin/OneDrive - Department of Education/Documents/Visual Studio Projects/MySchool/venv/Scripts/python.exe" "c:\Users\kevin\OneDrive - Department of Education\Documents\Visual Studio Projects\MySchool\modules\timetable_to_json_fixed.py" "%TEXT_PATH%"

if %ERRORLEVEL% NEQ 0 (
    echo Failed to convert timetable text to JSON.
    exit /b %ERRORLEVEL%
)

exit /b 0
