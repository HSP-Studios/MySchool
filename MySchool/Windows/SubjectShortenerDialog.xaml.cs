using System.Windows;
using System.Windows.Controls;

namespace MySchool.Windows
{
    public partial class SubjectShortenerDialog : Window
    {
        private readonly Dictionary<string, TextBox> subjectTextBoxes = new();
        public Dictionary<string, string> ShortenedSubjects { get; private set; } = new();

        public SubjectShortenerDialog(List<string> longSubjects)
        {
            InitializeComponent();
            PopulateSubjects(longSubjects);
        }

        private void PopulateSubjects(List<string> longSubjects)
        {
            int breakCounter = 1;

            foreach (var subject in longSubjects)
            {
                // Create a grid for each subject
                var grid = new Grid
                {
                    Margin = new Thickness(0, 0, 0, 12)
                };
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Original subject name (read-only)
                var originalTextBox = new TextBox
                {
                    Text = subject,
                    IsReadOnly = true,
                    Background = System.Windows.Media.Brushes.Transparent,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(12, 8, 12, 8),
                    FontSize = 14,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                originalTextBox.SetResourceReference(TextBox.BorderBrushProperty, "Brush.Border");
                originalTextBox.SetResourceReference(TextBox.ForegroundProperty, "Brush.TextSecondary");
                Grid.SetColumn(originalTextBox, 0);
                grid.Children.Add(originalTextBox);

                // Arrow icon
                var arrowText = new TextBlock
                {
                    Text = "->",
                    FontSize = 18,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                arrowText.SetResourceReference(TextBlock.ForegroundProperty, "Brush.Primary");
                Grid.SetColumn(arrowText, 1);
                grid.Children.Add(arrowText);

                // Determine shortened name - auto-shorten breaks
                string shortenedName;
                if (IsBreakName(subject))
                {
                    shortenedName = $"Break {breakCounter}";
                    breakCounter++;
                }
                else
                {
                    shortenedName = subject.Length > 8 ? subject.Substring(0, 8) : subject;
                }

                // Shortened subject name (editable)
                var shortenedTextBox = new TextBox
                {
                    Text = shortenedName,
                    MaxLength = 8,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(12, 8, 12, 8),
                    FontSize = 14,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                shortenedTextBox.SetResourceReference(TextBox.BorderBrushProperty, "Brush.Primary");
                shortenedTextBox.SetResourceReference(TextBox.ForegroundProperty, "Brush.TextPrimary");
                shortenedTextBox.SetResourceReference(TextBox.BackgroundProperty, "Brush.Background");
                Grid.SetColumn(shortenedTextBox, 2);
                grid.Children.Add(shortenedTextBox);

                // Store reference to the textbox
                subjectTextBoxes[subject] = shortenedTextBox;

                SubjectsPanel.Children.Add(grid);
            }
        }

        private bool IsBreakName(string subjectName)
        {
            var lowerSubject = subjectName.ToLower();
            return lowerSubject.Contains("break") ||
                   lowerSubject.Contains("recess") ||
                   lowerSubject.Contains("lunch") ||
                   lowerSubject.Contains("morning tea") ||
                   lowerSubject.Contains("afternoon tea");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate that all shortened names are not empty and within 8 characters
            var errors = new List<string>();

            foreach (var kvp in subjectTextBoxes)
            {
                var originalName = kvp.Key;
                var shortenedName = kvp.Value.Text.Trim();

                if (string.IsNullOrWhiteSpace(shortenedName))
                {
                    errors.Add($"'{originalName}' cannot be empty.");
                }
                else if (shortenedName.Length > 8)
                {
                    errors.Add($"'{originalName}' shortened name is still too long ({shortenedName.Length} characters).");
                }
                else
                {
                    ShortenedSubjects[originalName] = shortenedName;
                }
            }

            if (errors.Any())
            {
                MessageBox.Show(
                    string.Join("\n", errors),
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
