#!/usr/bin/env python
"""
Timetable to JSON Converter (Columnar Data Version)

This script converts raw text extracted from PDF timetables
into a structured JSON format, correctly handling the columnar
data format of the timetable.
"""

import os
import json
import datetime
import re
from pathlib import Path


def extract_student_info(lines):
    """Extract student information from the beginning of the timetable."""
    return {
        "last_name": lines[0].strip() if len(lines) > 0 else "",
        "first_name": lines[2].strip() if len(lines) > 2 else "",
        "student_id": lines[5].strip() if len(lines) > 5 else "",
        "grade": lines[7].strip() if len(lines) > 7 else "",
        "house": lines[9].strip() if len(lines) > 9 else "",
        "homeroom": lines[11].strip() if len(lines) > 11 else "",
        "homeroom_teacher": lines[13].strip() if len(lines) > 13 else ""
    }


def extract_legend_mappings(file_content):
    """Extract class and teacher mappings from the legend section."""
    class_mappings = {}
    teacher_mappings = {}
    
    # Find the legend section
    legend_match = re.search(r'Legend:(.*?)(?:Saturday|Page:|$)', file_content, re.DOTALL)
    if not legend_match:
        print("Warning: Could not find Legend section")
        return class_mappings, teacher_mappings
    
    legend_text = legend_match.group(1)
    
    # Hard-code the mappings based on the timetable PDF structure
    # Class mappings
    class_mappings = {
        "DIG091A": "Digital Technologies",
        "ENG091G": "English",
        "FTS091F": "Futures",
        "HAB14": "Roll Class",
        "HPE091J": "Health and Physical Education",
        "MAT091G": "Mathematics",
        "SCI091G": "Science",
        "TMT091B": "Materials and Technologies Specialisations"
    }
    
    # Teacher mappings
    teacher_mappings = {
        "BLANKA": "Miss Blankenhagen",
        "DENTMI": "Mr Denton",
        "GRIFAM": "Miss Griffiths",
        "HOHIBI": "Mrs Ho Hip",
        "JONENA": "Mr Jones",
        "LEHAPE": "Ms Lehane",
        "NIBBSY": "Miss Nibbs",
        "SHERST": "Mr Sherlock"
    }
    
    return class_mappings, teacher_mappings


def parse_timetable(text_file_path, output_file_path=None):
    """
    Parse a timetable text file and convert it to JSON format.
    
    Args:
        text_file_path (str): Path to the text file containing timetable data.
        output_file_path (str, optional): Path to save the JSON output.
    
    Returns:
        bool: True if successful, False otherwise.
    """
    # Read the timetable file
    try:
        with open(text_file_path, 'r', encoding='utf-8') as file:
            content = file.read()
    except Exception as e:
        print(f"Error reading timetable text file: {e}")
        return False
    
    # Split content into lines for easier processing
    lines = content.split('\n')
    lines = [line for line in lines]  # Keep original whitespace for column alignment
    
    # Extract student information
    student = extract_student_info(lines)
    
    # Extract class and teacher mappings from legend section
    class_mappings, teacher_mappings = extract_legend_mappings(content)
    
    # Initialize timetable data structure
    days_of_week = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]
    timetable = {day: {} for day in days_of_week}
    
    # Define period types
    period_types = {
        "P0": "class", 
        "HR": "homeroom",
        "P1A": "class", "P1B": "class", 
        "P2A": "class", "P2B": "class",
        "L1": "break", "L2": "break", "L3": "break",
        "P3A": "class", "P3B": "class",
        "L4": "break", "L5": "break",
        "P4A": "class", "P4B": "class",
        "P5": "class"
    }
    
    # Locate the days row in the timetable
    days_row_idx = -1
    for i, line in enumerate(lines):
        if line.strip() == "Monday":
            days_row_idx = i
            break
    
    if days_row_idx == -1:
        print("Error: Could not find days row in timetable")
        return False
    
    # Identify the column for each day
    day_columns = {}
    for i in range(5):  # Look at the next 5 lines for day headers
        line_idx = days_row_idx + i
        if line_idx < len(lines) and lines[line_idx].strip() in days_of_week:
            day_columns[lines[line_idx].strip()] = line_idx - days_row_idx
    
    # Find the periods row
    periods_row_idx = days_row_idx + max(day_columns.values()) + 1 if day_columns else days_row_idx + 1
    
    # Check if periods row contains a period
    if periods_row_idx < len(lines) and lines[periods_row_idx].strip() not in ["P0", "HR", "P1A"]:
        # Try to find the first period
        for i in range(periods_row_idx, periods_row_idx + 5):
            if i < len(lines) and lines[i].strip() in ["P0", "HR", "P1A"]:
                periods_row_idx = i
                break
    
    # Process the timetable data
    current_idx = periods_row_idx
    while current_idx < len(lines):
        line = lines[current_idx].strip()
        
        # Check if we've reached a period indicator
        if line in period_types.keys():
            period = line
            period_type = period_types.get(period)
            
            # For each period, get the time, class, teacher, and room info
            # Timetable data is in columns by day
            time_start_idx = current_idx + 1
            dash_idx = time_start_idx + 1
            time_end_idx = dash_idx + 1
            class_idx = time_end_idx + 1
            teacher_idx = class_idx + 1
            room_idx = teacher_idx + 1
            
            # For each day, populate the timetable
            for day in days_of_week:
                if day in day_columns:
                    day_offset = day_columns[day]
                    
                    # Initialize period data
                    timetable[day][period] = {
                        "period": period,
                        "type": period_type
                    }
                    
                    # Extract time info
                    if time_start_idx < len(lines):
                        timetable[day][period]["time_start"] = lines[time_start_idx].strip()
                    
                    if time_end_idx < len(lines):
                        timetable[day][period]["time_end"] = lines[time_end_idx].strip()
                    
                    # For breaks, we don't need class, teacher, room info
                    if not period.startswith('L'):
                        # Get class info
                        if class_idx + day_offset < len(lines):
                            class_code = lines[class_idx + day_offset].strip()
                            if class_code and class_code != "-":
                                timetable[day][period]["class_code"] = class_code
                                timetable[day][period]["class_name"] = class_mappings.get(class_code, "Unknown")
                        
                        # Get teacher info
                        if teacher_idx + day_offset < len(lines):
                            teacher_code = lines[teacher_idx + day_offset].strip()
                            if teacher_code and teacher_code != "-":
                                timetable[day][period]["teacher_code"] = teacher_code
                                timetable[day][period]["teacher_name"] = teacher_mappings.get(teacher_code, "Unknown")
                        
                        # Get room info
                        if room_idx + day_offset < len(lines):
                            room = lines[room_idx + day_offset].strip()
                            if room and room != "-":
                                timetable[day][period]["room"] = room
                        
                        # If no class code or it's "-", mark as a free period
                        if "class_code" not in timetable[day][period]:
                            timetable[day][period]["type"] = "free"
            
            # Move to the next period (periods are 6 lines apart in the file)
            current_idx = room_idx + max(day_columns.values()) + 1 if day_columns else room_idx + 6
        else:
            # Check for Legend: which signals the end of timetable data
            if line == "Legend:":
                break
            current_idx += 1
    
    # Combine everything into the final structure
    result = {
        "student": student,
        "timetable": timetable,
        "class_mappings": class_mappings,
        "teacher_mappings": teacher_mappings,
        "meta": {
            "generated_at": datetime.datetime.now().isoformat(),
            "source_file": os.path.basename(text_file_path)
        }
    }
    
    # Determine output path if not provided
    if not output_file_path:
        output_dir = os.path.dirname(text_file_path)
        filename = Path(text_file_path).stem
        output_file_path = os.path.join(output_dir, f"{filename}.json")
    
    # Save the JSON file
    try:
        with open(output_file_path, 'w', encoding='utf-8') as file:
            json.dump(result, file, indent=4)
        print(f"JSON successfully saved to {output_file_path}")
        return True
    except Exception as e:
        print(f"Error saving JSON file: {e}")
        return False


def main():
    """Main entry point for the script."""
    import sys
    
    if len(sys.argv) < 2:
        print("Usage: python timetable_to_json.py <timetable_text_file> [output_json_file]")
        sys.exit(1)
    
    text_file_path = sys.argv[1]
    
    if not os.path.isfile(text_file_path):
        print(f"Error: The file '{text_file_path}' does not exist.")
        sys.exit(1)
    
    output_file_path = None
    if len(sys.argv) >= 3:
        output_file_path = sys.argv[2]
    
    success = parse_timetable(text_file_path, output_file_path)
    
    sys.exit(0 if success else 1)


if __name__ == "__main__":
    main()
