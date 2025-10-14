using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySchool.Pages;

namespace MySchool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        
        // Navigate to Home page by default
        ContentFrame.Navigate(new HomePage());
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Apply Mica backdrop effect
        ApplyMicaBackdrop();
    }

    private void ApplyMicaBackdrop()
    {
        try
        {
            var windowHandle = new WindowInteropHelper(this).Handle;
            
            // Enable Mica backdrop for Windows 11
            var backdropType = 2; // DWMSBT_MAINWINDOW (Mica)
            DwmSetWindowAttribute(windowHandle, 38, ref backdropType, sizeof(int)); // DWMWA_SYSTEMBACKDROP_TYPE
            
            // Extend frame into client area
            var margins = new MARGINS { cxLeftWidth = -1, cxRightWidth = -1, cyTopHeight = -1, cyBottomHeight = -1 };
            DwmExtendFrameIntoClientArea(windowHandle, ref margins);
        }
        catch
        {
            // Fallback if Mica is not supported
        }
    }

    private void NavigationButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton button)
        {
            switch (button.Name)
            {
                case "HomeNavButton":
                    ContentFrame.Navigate(new HomePage());
                    break;
                case "ChatNavButton":
                    ContentFrame.Navigate(new ChatPage());
                    break;
                case "ScheduleNavButton":
                    ContentFrame.Navigate(new SchedulePage());
                    break;
                case "ResourcesNavButton":
                    ContentFrame.Navigate(new ResourcesPage());
                    break;
            }
        }
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    // DWM API declarations for Mica backdrop
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

    [StructLayout(LayoutKind.Sequential)]
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }
}