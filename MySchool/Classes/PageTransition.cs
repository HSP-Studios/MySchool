using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MySchool.Classes
{
    public static class PageTransition
    {
        public static void AnimatePageTransition(Frame frame, Page newPage, double duration = 0.3)
        {
            // Fade out current page
            if (frame.Content is Page currentPage)
            {
                var fadeOut = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    Duration = TimeSpan.FromSeconds(duration / 2),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
                };

                fadeOut.Completed += (s, e) =>
                {
                    // Navigate to new page
                    frame.Content = newPage;

                    // Fade in new page
                    newPage.Opacity = 0;
                    var fadeIn = new DoubleAnimation
                    {
                        From = 0.0,
                        To = 1.0,
                        Duration = TimeSpan.FromSeconds(duration / 2),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
                    };

                    newPage.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };

                currentPage.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
            else
            {
                // No current page, just fade in the new page
                frame.Content = newPage;
                newPage.Opacity = 0;
                var fadeIn = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(duration),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
                };

                newPage.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }
        }
    }
}
