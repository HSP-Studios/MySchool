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

        private void Schedule_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTimetablePdf();
        }

        private void LoadTimetablePdf()
        {
            try
            {
                var pdfPath = TimetableManager.GetLatestTimetablePdf();
                
                if (!string.IsNullOrEmpty(pdfPath) && File.Exists(pdfPath))
                {
                    // Show PDF viewer, hide no timetable message
                    NoTimetablePanel.Visibility = Visibility.Collapsed;
                    PdfViewer.Visibility = Visibility.Visible;
                    
                    // Navigate to the PDF file
                    PdfViewer.Navigate(new Uri(pdfPath));
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
