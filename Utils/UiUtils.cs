using DLClip.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using CheckBox = System.Windows.Controls.CheckBox;
using System.Diagnostics;

namespace DLClip.Utils
{
    internal static class UiUtils
    {
        public static bool HandleValidation(ValidationResult result)
        {
            if (result.Severity == ValidationSeverity.Failure)
            {
                MessageBox.Show(result.Message, result.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (result.Severity == ValidationSeverity.Warning)
            {
                MessageBox.Show(result.Message, result.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            else
            {
                return true;
            }

        }

        public static void UpdateUrlAvailabilityUi(CheckBox importFromUrlCheckbox, bool isLoaded, bool ytdlpOk)
        {
            if (isLoaded)
            {
                importFromUrlCheckbox.IsEnabled = false;
            } else
            {
                if (!ytdlpOk)
                {
                    importFromUrlCheckbox.IsChecked = false;
                    importFromUrlCheckbox.IsEnabled = false;
                } else
                {
                    importFromUrlCheckbox.IsEnabled = true;
                }
            }
        }

        public static void OpenUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }
    }
}