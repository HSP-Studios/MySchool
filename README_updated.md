# School Timetable Tools

This project contains tools for working with PDF timetables in a school system. It includes utilities for extracting text from PDF files and converting the extracted text into structured JSON format.

## Requirements

- Python 3.x
- PyPDF2 library (installed automatically with setup)

## Setup

The virtual environment and required dependencies are already set up. If you need to reinstall dependencies, run:

```
pip install -r requirements.txt
```

## Features

1. **PDF Text Extraction**: Extract raw text from PDF timetable files
2. **Text to JSON Conversion**: Convert extracted text into structured JSON format
3. **All-in-one PDF to JSON**: Convert directly from PDF to JSON in one step

## Usage

### PDF Text Extraction

#### Using Batch File (Windows CMD)

```
extract_pdf.bat path\to\your\file.pdf
```

#### Using PowerShell Script

```
.\extract_pdf.ps1 -PdfPath path\to\your\file.pdf
```

### Text to JSON Conversion

#### Using PowerShell Script (Recommended)

```
.\convert_timetable_fixed.ps1 -TextPath path\to\your\text\file.txt
```

#### Using Batch File

```
convert_timetable_fixed.bat path\to\your\text\file.txt
```

### All-in-one PDF to JSON Conversion

```
.\pdf_to_json_fixed.ps1 -PdfPath path\to\your\file.pdf
```

## Output

- The extracted text will be saved in the `output` folder with the `.txt` extension
- The generated JSON will be saved in the `output` folder with the `.json` extension

## JSON Structure

The generated JSON contains:

```json
{
  "student": {
    "last_name": "Student's last name",
    "first_name": "Student's first name",
    "student_id": "Student ID number",
    "grade": "Grade/Year level",
    "house": "House name",
    "homeroom": "Homeroom code",
    "homeroom_teacher": "Homeroom teacher"
  },
  "timetable": {
    "Monday": {
      "P1A": {
        "period": "P1A",
        "time_start": "9:00",
        "time_end": "9:35",
        "class_code": "MAT091G",
        "class_name": "Mathematics",
        "teacher_code": "TEACHR",
        "teacher_name": "Teacher Name",
        "room": "Room number",
        "type": "class"
      },
      // Other periods...
    },
    // Other days...
  },
  "class_mappings": {
    // Class code to name mappings
  },
  "teacher_mappings": {
    // Teacher code to name mappings
  },
  "meta": {
    "generated_at": "Timestamp",
    "source_file": "Source filename"
  }
}
```
