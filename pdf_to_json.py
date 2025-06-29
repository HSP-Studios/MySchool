import PyPDF2
import os
import sys

def extract_text_from_pdf(pdf_path):
    """
    Extract text from a PDF file and return it as a list of lines.
    
    Args:
        pdf_path (str): Path to the PDF file
        
    Returns:
        list: List of strings, where each string is a line from the PDF
    """
    if not os.path.exists(pdf_path):
        print(f"Error: PDF file not found at {pdf_path}")
        return []
    
    try:
        # Open the PDF file
        with open(pdf_path, 'rb') as file:
            # Create a PDF reader object
            pdf_reader = PyPDF2.PdfReader(file)
            
            # Initialize an empty string to store all text
            text = ""
            
            # Extract text from each page and append to text string
            for page_num in range(len(pdf_reader.pages)):
                page = pdf_reader.pages[page_num]
                text += page.extract_text()
            
            # Split the text by newline character to get a list of lines
            lines = text.split('\n')
            
            return lines
    
    except Exception as e:
        print(f"Error extracting text from PDF: {e}")
        return []

if __name__ == "__main__":
    # Check if a PDF path is provided as a command-line argument
    if len(sys.argv) > 1:
        pdf = sys.argv[1]
    else:
        # Default PDF path for testing
        pdf = "samples/timetable1.pdf"  # Adjust this path as needed
    
    # Extract text as a list of lines
    pdf_lines = extract_text_from_pdf(pdf)
    
    # Print total number of lines
    print(f"Extracted {len(pdf_lines)} lines from the PDF")
    
    # You can now access any line using pdf_lines[index]
    # For example: pdf_lines[39] would give you line 39 (if it exists)

    