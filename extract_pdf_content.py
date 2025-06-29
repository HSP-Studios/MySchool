import pdfplumber
import os
import sys
import json
from PIL import Image
import io

def extract_text_from_pdf(pdf_path):
    """Extract all text from a PDF file."""
    all_text = []
    
    try:
        with pdfplumber.open(pdf_path) as pdf:
            print(f"PDF has {len(pdf.pages)} pages")
            
            # Only process the first page
            if len(pdf.pages) > 0:
                page = pdf.pages[0]  # Get first page
                text = page.extract_text()
                if text:
                    # Split text into lines and get only the first 3 lines
                    lines = text.split('\n')
                    if len(lines) >= 3:
                        limited_text = '\n'.join(lines[:3])
                        all_text.append(f"--- Page 1 (First 3 Lines) ---\n{limited_text}")
                    else:
                        all_text.append(f"--- Page 1 (Available Lines) ---\n{text}")
                else:
                    all_text.append(f"--- Page 1 ---\n[No text content found]")
    except Exception as e:
        print(f"Error extracting text: {e}")
        return None
    
    return "\n\n".join(all_text)

def extract_tables_from_pdf(pdf_path):
    """Extract all tables from a PDF file."""
    all_tables = []
    
    try:
        with pdfplumber.open(pdf_path) as pdf:
            for i, page in enumerate(pdf.pages):
                tables = page.extract_tables()
                if tables:
                    # Filter tables to remove empty rows
                    filtered_tables = []
                    for table_index, table in enumerate(tables):
                        # Improved filtering for empty rows - check if ALL values in a row are either None or ''
                        filtered_table = []
                        for row in table:
                            # Check if all cells in the row are empty/None
                            all_empty = True
                            for cell in row:
                                if cell is not None and cell != '':
                                    all_empty = False
                                    break
                            
                            # Only keep rows that aren't all empty
                            if not all_empty:
                                filtered_table.append(row)
                        
                        # Special handling for Table 1 on Page 1 (remove P0 and P5 rows)
                        if i == 0 and table_index == 0:  # First page, first table
                            # Filter out P0 and P5 rows more accurately
                            p0_p5_filtered = []
                            for row in filtered_table:
                                if row[0] is None or row[0] == '':
                                    p0_p5_filtered.append(row)  # Keep rows with None/empty first cell
                                elif isinstance(row[0], str):
                                    # Check if first cell starts with P0 or P5
                                    if not (row[0] == 'P0' or row[0].startswith('P0 ') or 
                                           row[0] == 'P5' or row[0].startswith('P5 ')):
                                        p0_p5_filtered.append(row)
                                else:
                                    p0_p5_filtered.append(row)  # Keep non-string values
                            
                            filtered_table = p0_p5_filtered
                        
                        if filtered_table:  # Only add if there are rows left after filtering
                            filtered_tables.append(filtered_table)
                    
                    if filtered_tables:  # Only add if there are tables after filtering
                        all_tables.append({
                            "page": i+1,
                            "tables": filtered_tables
                        })
    except Exception as e:
        print(f"Error extracting tables: {e}")
        return None
    
    return all_tables

def extract_charts_and_images(pdf_path, output_dir=None):
    """Extract charts and images from a PDF file."""
    if output_dir:
        os.makedirs(output_dir, exist_ok=True)
    
    images_info = []
    
    try:
        with pdfplumber.open(pdf_path) as pdf:
            for i, page in enumerate(pdf.pages):
                # Extract images from the page
                images = page.images
                if images:
                    for j, img in enumerate(images):
                        img_filename = None
                        if output_dir:
                            img_filename = f"page_{i+1}_image_{j+1}.png"
                            img_path = os.path.join(output_dir, img_filename)
                            
                            # Convert the image stream to PIL Image and save
                            try:
                                image = Image.open(io.BytesIO(img["stream"].get_data()))
                                image.save(img_path)
                            except Exception as e:
                                print(f"Error saving image: {e}")
                                img_filename = None
                        
                        images_info.append({
                            "page": i+1,
                            "image_index": j+1,
                            "width": img["width"],
                            "height": img["height"],
                            "x0": img["x0"],
                            "y0": img["y0"],
                            "x1": img["x1"],
                            "y1": img["y1"],
                            "filename": img_filename
                        })
    except Exception as e:
        print(f"Error extracting images: {e}")
        return None
    
    return images_info

def main():
    if len(sys.argv) < 2:
        print("Usage: python extract_pdf_content.py <pdf_file_path> [output_dir]")
        sys.exit(1)
    
    pdf_path = sys.argv[1]
    output_dir = sys.argv[2] if len(sys.argv) > 2 else "output"
    
    if not os.path.exists(pdf_path):
        print(f"Error: PDF file not found at {pdf_path}")
        sys.exit(1)
    
    print(f"Processing PDF: {pdf_path}")
    
    # Extract text
    text = extract_text_from_pdf(pdf_path)
    if text:
        print("\n=== TEXT CONTENT ===")
        print(text)
    
    # Extract tables
    tables = extract_tables_from_pdf(pdf_path)
    if tables:
        print("\n=== TABLES ===")
        for page_tables in tables:
            page_num = page_tables["page"]
            print(f"\nPage {page_num} Tables:")
            
            for i, table in enumerate(page_tables["tables"]):
                print(f"\nTable {i+1}:")
                for row in table:
                    print(row)

if __name__ == "__main__":
    main()
