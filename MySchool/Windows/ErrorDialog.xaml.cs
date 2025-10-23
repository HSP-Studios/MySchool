using System.Windows;

namespace MySchool.Windows
{
    /// <summary>
    /// Custom error dialog with "Open Logs Folder" and "Ok" buttons
    /// </summary>
    public partial class ErrorDialog : Window
    {
        public bool OpenLogsRequested { get; private set; }

        public ErrorDialog(string title, string message)
        {
            InitializeComponent();
            TitleTextBlock.Text = title;
            MessageTextBlock.Text = message;
        }

        private void OpenLogsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenLogsRequested = true;
            DialogResult = true;
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            OpenLogsRequested = false;
            DialogResult = true;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenLogsRequested = false;
            DialogResult = false;
            Close();
        }
    }
}
