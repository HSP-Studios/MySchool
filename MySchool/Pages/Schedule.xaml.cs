using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySchool.Classes;

namespace MySchool.Pages
{
    /// <summary>
    /// Interaction logic for Schedule.xaml
    /// </summary>
    public partial class Schedule : Page
    {
        public Schedule()
        {
            InitializeComponent();
            Loaded += Schedule_Loaded;
        }

        private async void Schedule_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTimetablePdf();
        }

        private async Task LoadTimetablePdf()
        {
            try
            {
                var pdfPath = TimetableManager.GetLatestTimetablePdf();
                
                if (!string.IsNullOrEmpty(pdfPath) && File.Exists(pdfPath))
                {
                    // Show PDF viewer, hide no timetable message
                    NoTimetablePanel.Visibility = Visibility.Collapsed;
                    PdfViewer.Visibility = Visibility.Visible;
                    
                    // Ensure WebView2 is initialized
                    await PdfViewer.EnsureCoreWebView2Async();
                    
                    // Navigate to the PDF file
                    PdfViewer.Source = new Uri(pdfPath);
                }
                else
                {
                    // Show no timetable message, hide PDF viewer
                    NoTimetablePanel.Visibility = Visibility.Visible;
                    PdfViewer.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load PDF: {ex.Message}");
                NoTimetablePanel.Visibility = Visibility.Visible;
                PdfViewer.Visibility = Visibility.Collapsed;
            }
        }
    }
}
