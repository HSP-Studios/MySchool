#!/usr/bin/env python3
"""
OneSchool Portal (OSLP) Interaction Script
Automates login and timetable download from OneSchool
"""

import os
import time
import getpass
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import Select
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager
from selenium.common.exceptions import TimeoutException, NoSuchElementException

class OneSchoolAutomator:
    def __init__(self, download_path=None):
        """Initialize the OneSchool automator"""
        self.base_url = "https://oslp.eq.edu.au"
        self.driver = None
        self.wait = None
        
        # Set up download path
        if download_path is None:
            current_dir = os.path.dirname(os.path.abspath(__file__))
            self.download_path = os.path.join(os.path.dirname(current_dir), "output")
        else:
            self.download_path = download_path
        
        # Ensure download directory exists
        os.makedirs(self.download_path, exist_ok=True)
    
    def setup_driver(self):
        """Set up Chrome driver with download preferences"""
        chrome_options = webdriver.ChromeOptions()
        
        # Set download preferences
        prefs = {
            "download.default_directory": self.download_path,
            "download.prompt_for_download": False,
            "download.directory_upgrade": True,
            "safebrowsing.enabled": True
        }
        chrome_options.add_experimental_option("prefs", prefs)
        
        # Optional: Run in headless mode (uncomment to hide browser window)
        # chrome_options.add_argument("--headless")
        
        # Set up the driver
        service = Service(ChromeDriverManager().install())
        self.driver = webdriver.Chrome(service=service, options=chrome_options)
        self.wait = WebDriverWait(self.driver, 20)
        
        print(f"Chrome driver initialized. Downloads will be saved to: {self.download_path}")
    
    def get_credentials(self):
        """Get username and password from user input"""
        print("Please enter your OneSchool credentials:")
        username = input("Username: ").strip()
        password = getpass.getpass("Password: ").strip()
        
        if not username or not password:
            raise ValueError("Username and password cannot be empty")
        
        return username, password
    
    def navigate_to_login(self):
        """Navigate to OneSchool and handle initial redirect"""
        print("Navigating to OneSchool...")
        self.driver.get(self.base_url)
        
        # Wait a moment for the page to load
        time.sleep(3)
        print(f"Current URL after initial load: {self.driver.current_url}")
        
        # Check if we're already on a login page or if we need to wait for redirect
        current_url = self.driver.current_url.lower()
        if "login" not in current_url and "sso" not in current_url:
            print("Waiting for redirect to login page...")
            try:
                # Wait for redirect to login page with extended timeout
                self.wait.until(lambda driver: "login" in driver.current_url.lower() or "sso" in driver.current_url.lower())
                print(f"Redirected to login page: {self.driver.current_url}")
            except TimeoutException:
                print(f"No automatic redirect detected. Current URL: {self.driver.current_url}")
                # Check if there's a login link or button to click
                try:
                    login_links = self.driver.find_elements(By.PARTIAL_LINK_TEXT, "Login")
                    if not login_links:
                        login_links = self.driver.find_elements(By.PARTIAL_LINK_TEXT, "Sign in")
                    if login_links:
                        print("Found login link, clicking...")
                        login_links[0].click()
                        time.sleep(3)
                        print(f"After clicking login link: {self.driver.current_url}")
                except NoSuchElementException:
                    print("No login link found")
                    
        else:
            print(f"Already on login page: {self.driver.current_url}")
    
    def login(self, username, password):
        """Perform login with provided credentials"""
        print("Looking for login form...")
        print(f"Current page title: {self.driver.title}")
        print(f"Current URL: {self.driver.current_url}")
        
        try:
            # Wait for page to fully load
            time.sleep(2)
            
            # Enter username
            print("Looking for username field...")
            username_field = self.wait.until(EC.presence_of_element_located((By.ID, "username")))
            username_field.clear()
            username_field.send_keys(username)
            print("Username entered")
            
            # Enter password
            print("Looking for password field...")
            password_field = self.wait.until(EC.presence_of_element_located((By.ID, "password")))
            password_field.clear()
            password_field.send_keys(password)
            print("Password entered")
            
            # Accept terms and conditions
            print("Looking for terms and conditions checkbox...")
            try:
                terms_checkbox = self.wait.until(EC.element_to_be_clickable((By.ID, "sso-cou")))
                if not terms_checkbox.is_selected():
                    terms_checkbox.click()
                    print("Terms and conditions accepted")
                else:
                    print("Terms and conditions already checked")
            except TimeoutException:
                print("Terms and conditions checkbox not found - it might not be required")
            
            # Click submit button
            print("Looking for submit button...")
            submit_button = self.wait.until(EC.element_to_be_clickable((By.ID, "sso-signin")))
            submit_button.click()
            print("Login submitted")
            
            # Wait for redirect to welcome page
            print("Waiting for login to complete...")
            try:
                # Create a longer wait for login completion
                long_wait = WebDriverWait(self.driver, 30)
                long_wait.until(lambda driver: "welcome.aspx" in driver.current_url)
                print("Successfully logged in and redirected to welcome page")
            except TimeoutException:
                print(f"Login may have failed or taken longer than expected. Current URL: {self.driver.current_url}")
                # Check if we're on an error page or still on login
                if "login" in self.driver.current_url.lower() or "error" in self.driver.current_url.lower():
                    print("Still on login page - login may have failed")
                    # Look for error messages
                    try:
                        error_elements = self.driver.find_elements(By.CLASS_NAME, "error")
                        if not error_elements:
                            error_elements = self.driver.find_elements(By.XPATH, "//*[contains(text(), 'error') or contains(text(), 'invalid')]")
                        if error_elements:
                            print(f"Error message found: {error_elements[0].text}")
                    except:
                        pass
                    raise Exception("Login failed - please check credentials")
                else:
                    print("Login appears successful but didn't reach expected welcome page")
            
        except TimeoutException as e:
            print(f"Timeout during login: {e}")
            print(f"Current page source (first 500 chars): {self.driver.page_source[:500]}")
            raise
        except NoSuchElementException as e:
            print(f"Element not found during login: {e}")
            print(f"Current page source (first 500 chars): {self.driver.page_source[:500]}")
            raise
    
    def navigate_to_timetable(self):
        """Navigate to student timetable section"""
        print("Navigating to student timetable...")
        
        try:
            # Find and select the reports dropdown
            reports_dropdown = self.wait.until(
                EC.presence_of_element_located((By.ID, "b_a_ocph_ocph_rlbReportsRf_NoSetP_ddlReports"))
            )
            
            select = Select(reports_dropdown)
            select.select_by_value("3374")  # Student Timetable - Weekly
            print("Selected 'Student Timetable - Weekly' from dropdown")
            
            # Store the current number of tabs before clicking Go
            initial_tabs = len(self.driver.window_handles)
            print(f"Current number of tabs: {initial_tabs}")
            
            # Click the Go button
            go_button = self.wait.until(
                EC.element_to_be_clickable((By.ID, "b_a_ocph_ocph_rlbReportsRf_NoSetP_btnRunReport"))
            )
            go_button.click()
            print("Clicked Go button")
            
            # Wait for new tab to open
            print("Waiting for new tab to open...")
            start_time = time.time()
            while time.time() - start_time < 5:  # 5 second timeout
                current_tabs = len(self.driver.window_handles)
                if current_tabs > initial_tabs:
                    print(f"New tab opened! Now have {current_tabs} tabs")
                    # Switch to the new tab (last one in the list)
                    new_tab = self.driver.window_handles[-1]
                    self.driver.switch_to.window(new_tab)
                    print("Switched to new tab")
                    break
                time.sleep(0.2)  # Check every 200ms
            else:
                print("No new tab detected, checking current tab...")
            
            # Now wait for the PDF page to load in the current tab
            print("Waiting for PDF page to load...")
            start_time = time.time()
            while time.time() - start_time < 10:
                current_url = self.driver.current_url
                page_title = self.driver.title
                print(f"Current URL: {current_url}")
                print(f"Page title: {page_title}")
                
                if "ReportViewerScheduler.aspx" in current_url or "TTStudentTimetable" in page_title:
                    print("Successfully reached report viewer page")
                    return
                    
                time.sleep(0.5)  # Check every half second
            
            print("Timeout waiting for report viewer page, but continuing...")
            print(f"Final URL: {self.driver.current_url}")
            print(f"Final title: {self.driver.title}")
            
        except TimeoutException as e:
            print(f"Timeout during timetable navigation: {e}")
            raise
        except NoSuchElementException as e:
            print(f"Element not found during timetable navigation: {e}")
            raise
    
    def download_timetable(self):
        """Download the timetable PDF"""
        print("Downloading timetable PDF...")
        
        try:
            # Wait a moment for the page to fully load
            print("Waiting for page to fully load...")
            time.sleep(5)
            
            current_url = self.driver.current_url
            print(f"Current URL: {current_url}")
            print(f"Page title: {self.driver.title}")
            
            # Wait additional time for the PDF to render
            print("Waiting additional time before looking for download elements...")
            time.sleep(3)
            
            # Look for iframe containing the PDF
            try:
                iframes = self.driver.find_elements(By.TAG_NAME, "iframe")
                if iframes:
                    print(f"Found {len(iframes)} iframe(s)")
                    for i, iframe in enumerate(iframes):
                        src = iframe.get_attribute("src")
                        print(f"Iframe {i+1} src: {src}")
                        if src and ('.pdf' in src.lower() or 'report' in src.lower()):
                            print(f"PDF iframe found: {src}")
                            # Navigate directly to the PDF URL
                            self.driver.get(src)
                            time.sleep(3)
                            break
            except Exception as e:
                print(f"Error checking for iframes: {e}")
            
            # Try different download approaches
            try:
                # Method 1: Look for download buttons with more selectors
                download_selectors = [
                    "//button[contains(@title, 'Download')]",
                    "//button[contains(@aria-label, 'Download')]", 
                    "//a[contains(@title, 'Download')]",
                    "//a[contains(@href, 'download')]",
                    "//*[@class='download']",
                    "//*[contains(@class, 'download')]",
                    "//button[contains(text(), 'Download')]",
                    "//input[@value='Download']",
                    "//*[contains(@onclick, 'download')]",
                    "//a[contains(@href, '.pdf')]"
                ]
                
                download_element = None
                for selector in download_selectors:
                    try:
                        elements = self.driver.find_elements(By.XPATH, selector)
                        if elements:
                            download_element = elements[0]
                            print(f"Found download element with selector: {selector}")
                            break
                    except:
                        continue
                
                if download_element:
                    download_element.click()
                    print("Clicked download button")
                    time.sleep(3)
                else:
                    # Method 2: Try right-click and save
                    print("No download button found, trying right-click context menu...")
                    from selenium.webdriver.common.keys import Keys
                    from selenium.webdriver.common.action_chains import ActionChains
                    
                    actions = ActionChains(self.driver)
                    # Right click to open context menu
                    actions.context_click().perform()
                    time.sleep(1)
                    # Press 'A' for Save As (may vary by browser language)
                    actions.send_keys('a').perform()
                    time.sleep(2)
                    
                    # If that doesn't work, try Ctrl+S
                    if not download_element:
                        print("Trying Ctrl+S to save...")
                        actions = ActionChains(self.driver)
                        actions.key_down(Keys.CONTROL).send_keys('s').key_up(Keys.CONTROL).perform()
                        time.sleep(3)
                
            except Exception as e:
                print(f"Error with download methods: {e}")
            
            # Method 3: Try to access the PDF URL directly if available
            current_url = self.driver.current_url
            if current_url.endswith('.pdf') or 'pdf' in current_url.lower():
                print("Direct PDF URL detected, navigating to trigger download...")
                self.driver.get(current_url)
                time.sleep(3)
            
            # Wait for download to complete (check download directory)
            print("Waiting for download to complete...")
            download_timeout = 30
            start_time = time.time()
            initial_files = set(os.listdir(self.download_path))
            
            while time.time() - start_time < download_timeout:
                current_files = set(os.listdir(self.download_path))
                new_files = current_files - initial_files
                
                # Check for completed PDF files (not partial downloads)
                pdf_files = [f for f in new_files if f.endswith('.pdf') and not f.endswith('.crdownload')]
                if pdf_files:
                    print(f"Download completed: {pdf_files[0]}")
                    return pdf_files[0]
                
                # Also check for any new files that might be the PDF
                if new_files:
                    for new_file in new_files:
                        if not new_file.endswith('.crdownload'):  # Not a partial download
                            print(f"New file detected: {new_file}")
                            # Check if it's actually a PDF by trying to rename it
                            if not new_file.endswith('.pdf'):
                                try:
                                    old_path = os.path.join(self.download_path, new_file)
                                    new_path = os.path.join(self.download_path, f"{new_file}.pdf")
                                    os.rename(old_path, new_path)
                                    print(f"Renamed {new_file} to {new_file}.pdf")
                                    return f"{new_file}.pdf"
                                except:
                                    pass
                            else:
                                return new_file
                
                time.sleep(1)
            
            print("Download timeout reached. Checking for any new files...")
            current_files = set(os.listdir(self.download_path))
            new_files = current_files - initial_files
            if new_files:
                print(f"New files found: {list(new_files)}")
                return list(new_files)[0]
            else:
                print("No new files detected in download directory")
                return None
                
        except Exception as e:
            print(f"Error during download: {e}")
            raise
    
    def run(self):
        """Main execution method"""
        try:
            # Get credentials
            username, password = self.get_credentials()
            
            # Set up browser
            self.setup_driver()
            
            # Navigate and login
            self.navigate_to_login()
            self.login(username, password)
            
            # Navigate to timetable and download
            self.navigate_to_timetable()
            downloaded_file = self.download_timetable()
            
            if downloaded_file:
                print(f"Successfully downloaded timetable: {downloaded_file}")
                print(f"File saved to: {self.download_path}")
            else:
                print("Download completed, but file name could not be determined")
            
            # Wait a moment before closing
            time.sleep(2)
            
        except Exception as e:
            print(f"Error: {e}")
            raise
        finally:
            if self.driver:
                self.driver.quit()
                print("Browser closed")

def main():
    """Main function to run the OneSchool automator"""
    automator = OneSchoolAutomator()
    automator.run()

if __name__ == "__main__":
    main()