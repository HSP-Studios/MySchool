@echo off
rem PDF Text Extractor Wrapper Script
rem Usage: extract_pdf.bat <pdf_file_path>

if "%1"=="" (
    echo Error: No PDF file provided.
    echo Usage: extract_pdf.bat ^<pdf_file_path^>
    exit /b 1
)

set PDF_PATH=%1

echo Extracting text from %PDF_PATH%...
"C:/Users/kevin/OneDrive - Department of Education/Documents/Visual Studio Projects/MySchool/venv/Scripts/python.exe" "c:\Users\kevin\OneDrive - Department of Education\Documents\Visual Studio Projects\MySchool\modules\pdf_text_extractor.py" "%PDF_PATH%"

if %ERRORLEVEL% NEQ 0 (
    echo Failed to extract text from PDF.
    exit /b %ERRORLEVEL%
)

exit /b 0
