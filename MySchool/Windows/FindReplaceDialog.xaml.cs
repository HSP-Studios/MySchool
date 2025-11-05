using System.Windows;

namespace MySchool.Windows
{
    public partial class FindReplaceDialog : Window
    {
    public string FindText { get; private set; } = string.Empty;
        public string ReplaceText { get; private set; } = string.Empty;
 public bool ShouldReplace { get; private set; } = false;

     public FindReplaceDialog()
        {
 InitializeComponent();
        }

     private void ReplaceButton_Click(object sender, RoutedEventArgs e)
      {
       FindText = FindTextBox.Text.Trim();
            ReplaceText = ReplaceTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(FindText))
  {
        MessageBox.Show(
          "Please enter a subject name to find.",
         "Validation Error",
              MessageBoxButton.OK,
             MessageBoxImage.Warning);
       return;
            }

if (string.IsNullOrWhiteSpace(ReplaceText))
 {
        var result = MessageBox.Show(
        "Replace text is empty. This will clear the subject name. Continue?",
    "Confirm Replace",
        MessageBoxButton.YesNo,
      MessageBoxImage.Question);

  if (result == MessageBoxResult.No)
       {
              return;
        }
            }

            ShouldReplace = true;
    DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
   {
      ShouldReplace = false;
            DialogResult = false;
Close();
}

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
 CancelButton_Click(sender, e);
        }
    }
}
