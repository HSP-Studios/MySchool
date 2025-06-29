#!/usr/bin/env python
"""
Timetable to JSON Converter

This script converts raw text extracted from PDF timetables
into a structured JSON format.
"""

import os
import json
import datetime
import re
from pathlib import Path
import datetime
import re
from pathlib import Path


def extract_student_info(lines):
    """Extract student information from the beginning of the timetable."""
    return {
        "last_name": lines[0].strip(),
        "first_name": lines[2].strip(),
        "student_id": lines[5].strip(),
        "grade": lines[7].strip(),
        "house": lines[9].strip(),
        "homeroom": lines[11].strip(),
        "homeroom_teacher": lines[13].strip()
    }


def extract_class_teacher_mappings(lines):
    """Extract class and teacher mappings from the Legend section."""
    class_mappings = {}
    teacher_mappings = {}
    
    # Find the Legend section
    legend_index = -1
    for i, line in enumerate(lines):
        if line.strip() == "Legend:":
            legend_index = i
            break
    
    if legend_index == -1:
        print("Warning: Could not find Legend section")
        return class_mappings, teacher_mappings
    
    # Find headers
    class_header_index = -1
    teacher_header_index = -1
    
    for i in range(legend_index, len(lines)):
        if lines[i].strip() == "Class Code":
            class_header_index = i
        elif lines[i].strip() == "Teacher Code":
            teacher_header_index = i
            break
    
    if class_header_index == -1 or teacher_header_index == -1:
        print("Warning: Could not find Class Code or Teacher Code headers")
        return class_mappings, teacher_mappings
    
    # Process class mappings
    i = class_header_index + 2  # Skip "Class Code" and "Class Name"
    while i < teacher_header_index:
        if lines[i].strip() and i + 1 < len(lines) and lines[i+1].strip():
            # Use class code as key and class name as value
            class_code = lines[i].strip()
            class_name = lines[i+1].strip()
            
            # Special case for Materials and Technologies Specialisations
            if class_code == "Materials and Technologies" and lines[i+1].strip() == "Specialisations":
                # Find the class code from previous line
                if i > 0 and lines[i-1].strip():
                    class_code = lines[i-1].strip()
                    class_name = "Materials and Technologies Specialisations"
            else:
                class_mappings[class_code] = class_name
            i += 2
        else:
            i += 1
    
    # Process teacher mappings
    i = teacher_header_index + 2  # Skip "Teacher Code" and "Teacher"
    while i < len(lines) - 1:
        if "Saturday" in lines[i] or "Ref -" in lines[i]:
            break
            
        if lines[i].strip() and i + 1 < len(lines) and lines[i+1].strip():
            # Use teacher code as key and teacher name as value
            teacher_code = lines[i].strip()
            teacher_name = lines[i+1].strip()
            teacher_mappings[teacher_code] = teacher_name
            i += 2
        else:
            i += 1
            
    # Fix the Materials and Technologies Specialisations mapping
    if "TMT091B" in [line.strip() for line in lines] and "TMT091B" not in class_mappings:
        class_mappings["TMT091B"] = "Materials and Technologies Specialisations"
    
    return class_mappings, teacher_mappings


def parse_timetable_text(text_file_path):
    """
    Parse the raw text from a timetable PDF file.
    
    Args:
        text_file_path (str): Path to the text file containing the raw timetable text.
        
    Returns:
        dict: Structured timetable data.
    """
    try:
        with open(text_file_path, 'r', encoding='utf-8') as file:
            content = file.read()
    except Exception as e:
        print(f"Error reading timetable text file: {e}")
        return None
    
    # Split the file into lines for easier processing
    lines = content.split('\n')
    lines = [line.strip() for line in lines]
    
    # Extract student information
    student = extract_student_info(lines)
    
    # Define days of week
    days_of_week = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]
    
    # Extract class and teacher mappings
    class_mappings, teacher_mappings = extract_class_teacher_mappings(lines)
    
    # Find Legend section to identify the end of timetable data
    legend_index = -1
    for i, line in enumerate(lines):
        if line.strip() == "Legend:":
            legend_index = i
            break
    
    if legend_index == -1:
        legend_index = len(lines)  # If no legend section, use end of file
    
    # Find where the timetable data starts
    days_row = -1
    for i in range(15, 25):
        if i < len(lines) and lines[i].strip() == "Monday":
            days_row = i
            break
    
    if days_row == -1:
        print("Error: Could not find the start of timetable data")
        return None
    
    # Get the periods
    periods_row = -1
    for i in range(days_row + 1, days_row + 10):
        if i < len(lines) and lines[i].strip() == "P0":
            periods_row = i
            break
    
    if periods_row == -1:
        print("Error: Could not find periods in timetable")
        return None
    
    # Collect all the periods in the timetable
    periods = []
    i = periods_row
    while i < legend_index:
        period = lines[i].strip()
        if period and period in ["P0", "HR", "P1A", "P1B", "P2A", "P2B", "L1", "L2", "L3", "P3A", "P3B", "L4", "L5", "P4A", "P4B", "P5"]:
            periods.append(period)
        i += 1
        if i < len(lines) and re.match(r'^\d+:\d+$', lines[i].strip()):
            break
    
    # Initialize timetable structure
    timetable = {}
    for day in days_of_week:
        timetable[day] = {}
    
    # Create period types dictionary
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
    }        # Parse the timetable data by finding the starting position of each day's column
    row_data = {}
    # Locate day columns
    day_columns = {}
    for i, line in enumerate(lines[days_row:days_row+10]):
        for day_index, day in enumerate(days_of_week):
            if line.strip() == day:
                day_columns[day] = days_row + i
    
    if len(day_columns) != len(days_of_week):
        print(f"Error: Could not find column positions for all days. Found {len(day_columns)} of {len(days_of_week)}")
        return None
    
    # Identify where each period starts
    period_positions = []
    for i in range(periods_row, legend_index):
        if lines[i].strip() in ["P0", "HR", "P1A", "P1B", "P2A", "P2B", "L1", "L2", "L3", "P3A", "P3B", "L4", "L5", "P4A", "P4B", "P5"]:
            period_positions.append((i, lines[i].strip()))
    
    # Now extract data for each period
    for row_index, period in period_positions:
        period_type = period_types.get(period, "unknown")
        
        # Initialize this period for each day
        for day in days_of_week:
            timetable[day][period] = {
                "period": period,
                "type": period_type
            }
        
        # Get the next 6 rows (time start, dash, time end, class code, teacher code, room)
        time_start_idx = row_index + 1
        time_end_idx = row_index + 3
        class_code_idx = row_index + 4
        teacher_code_idx = row_index + 5
        room_idx = row_index + 6
          # Extract data for each day
        for day in days_of_week:
            # For breaks (lunch periods)
            if period.startswith("L"):
                # Process break periods - they usually just have time info
                if time_start_idx < legend_index:
                    timetable[day][period]["time_start"] = lines[time_start_idx].strip()
                if time_end_idx < legend_index:
                    timetable[day][period]["time_end"] = lines[time_end_idx].strip()
                timetable[day][period]["type"] = "break"
            else:
                # Regular class periods and homeroom
                # Extract time info
                if time_start_idx < legend_index:
                    timetable[day][period]["time_start"] = lines[time_start_idx].strip()
                if time_end_idx < legend_index:
                    timetable[day][period]["time_end"] = lines[time_end_idx].strip()
                
                # Look for class code, teacher code, and room in their respective rows
                # We need to find the correct column offset for each day
                day_offset = day_columns[day] - days_row
                
                # Extract class code
                if class_code_idx + day_offset < len(lines):
                    class_code = lines[class_code_idx + day_offset].strip()
                    if class_code and class_code != "-":
                        timetable[day][period]["class_code"] = class_code
                        timetable[day][period]["class_name"] = class_mappings.get(class_code, "Unknown")
                    
                # Extract teacher code
                if teacher_code_idx + day_offset < len(lines):
                    teacher_code = lines[teacher_code_idx + day_offset].strip()
                    if teacher_code and teacher_code != "-":
                        timetable[day][period]["teacher_code"] = teacher_code
                        timetable[day][period]["teacher_name"] = teacher_mappings.get(teacher_code, "Unknown")
                
                # Extract room
                if room_idx + day_offset < len(lines):
                    room = lines[room_idx + day_offset].strip()
                    if room and room != "-":
                        timetable[day][period]["room"] = room
                
                # If no class code or it's "-", mark as a free period
                if "class_code" not in timetable[day][period] or not timetable[day][period]["class_code"]:
                    timetable[day][period]["type"] = "free"
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
    
    return result


def save_json(data, output_path):
    """
    Save data as a JSON file.
    
    Args:
        data (dict): Data to save as JSON.
        output_path (str): Path to save the JSON file.
    """
    try:
        with open(output_path, 'w', encoding='utf-8') as file:
            json.dump(data, file, indent=4)
        print(f"JSON successfully saved to {output_path}")
        return True
    except Exception as e:
        print(f"Error saving JSON to file: {e}")
        return False


def convert_timetable_to_json(text_file_path, output_file_path=None):
    """
    Convert a timetable text file to JSON.
    
    Args:
        text_file_path (str): Path to the text file containing the raw timetable text.
        output_file_path (str, optional): Path to save the JSON file. If not provided,
            the JSON will be saved to the same directory as the text file with the same
            name but with a .json extension.
    
    Returns:
        bool: True if the conversion was successful, False otherwise.
    """
    if not output_file_path:
        output_dir = os.path.dirname(text_file_path)
        filename = Path(text_file_path).stem
        output_file_path = os.path.join(output_dir, f"{filename}.json")
    
    timetable_data = parse_timetable_text(text_file_path)
    
    if timetable_data:
        return save_json(timetable_data, output_file_path)
    else:
        return False


def main():
    """Main function to run the script."""
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
    
    success = convert_timetable_to_json(text_file_path, output_file_path)
    
    sys.exit(0 if success else 1)


if __name__ == "__main__":
    main()
