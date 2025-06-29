@echo off
rem Timetable to JSON Converter Batch Script
rem Usage: convert_timetable_v2.bat <timetable_text_file> [output_json_file]

if "%1"=="" (
    echo Error: No timetable text file provided.
    echo Usage: convert_timetable_v2.bat ^<timetable_text_file^> [output_json_file]
    exit /b 1
)

set TIMETABLE_PATH=%1

if "%2"=="" (
    for %%F in ("%TIMETABLE_PATH%") do set FILENAME=%%~nF
    for %%F in ("%TIMETABLE_PATH%") do set DIRECTORY=%%~dpF
    set OUTPUT_PATH=%DIRECTORY%%FILENAME%.json
) else (
    set OUTPUT_PATH=%2
)

echo Converting timetable from %TIMETABLE_PATH% to JSON...
"C:/Users/kevin/OneDrive - Department of Education/Documents/Visual Studio Projects/MySchool/venv/Scripts/python.exe" "modules/timetable_parser_v2.py" "%TIMETABLE_PATH%" "%OUTPUT_PATH%"

if %ERRORLEVEL% NEQ 0 (
    echo Conversion failed.
    exit /b %ERRORLEVEL%
)

echo Conversion successful. JSON saved to: %OUTPUT_PATH%
exit /b 0
