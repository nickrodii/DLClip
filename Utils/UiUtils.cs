using DLClip.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using CheckBox = System.Windows.Controls.CheckBox;
using System.Diagnostics;
using GroupBox = System.Windows.Controls.GroupBox;
using Label = System.Windows.Controls.Label;
using Button = System.Windows.Controls.Button;
using Application = System.Windows.Application;
using TextBox = System.Windows.Controls.TextBox;
using System.Windows.Controls;
using ValidationResult = DLClip.Models.ValidationResult;

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

        public static void SetLoadedUiState(bool load, GroupBox importBox, GroupBox trimLengthBox, GroupBox videoOptionsBox, GroupBox outputBox, Button loadButton, Label loadedLabel, CheckBox useUrlCheckbox, TextBox filePath, Button runButton)
        {
            var app = ((App)Application.Current);

            if (load)
            {
                importBox.IsEnabled = false;
                trimLengthBox.IsEnabled = true;
                videoOptionsBox.IsEnabled = true;
                outputBox.IsEnabled = true;
                runButton.IsEnabled = true;
                UiUtils.UpdateUrlAvailabilityUi(useUrlCheckbox, true, app.YtdlpOk);
                loadButton.Content = "Unload Media";
                loadedLabel.Content = "Successfully loaded!";
            } 
            
            else
            {
                importBox.IsEnabled = true;
                trimLengthBox.IsEnabled = false;
                videoOptionsBox.IsEnabled = false;
                outputBox.IsEnabled = false;
                runButton.IsEnabled = false;
                filePath.Text = string.Empty;
                UiUtils.UpdateUrlAvailabilityUi(useUrlCheckbox, false, app.YtdlpOk);
                loadButton.Content = "Load Media";
                loadedLabel.Content = "Not loaded yet...";
            }
        }

        public static void ShowTrimDefaults(Slider startSlider, Slider endSlider,TextBox startTime, TextBox endTime, Label length, TrimDefaults defaults )
        {
            startSlider.Minimum = 0;
            endSlider.Minimum = 0;
            startSlider.Maximum = defaults.DurationSeconds;
            endSlider.Maximum = defaults.DurationSeconds;

            startSlider.Value = defaults.StartSeconds;
            endSlider.Value = defaults.EndSeconds;

            startTime.Text = TimeUtils.FormatSecondsToTime((int)Math.Round(defaults.StartSeconds));
            endTime.Text = TimeUtils.FormatSecondsToTime((int)Math.Round(defaults.EndSeconds));
            double lengthValue = defaults.EndSeconds - defaults.StartSeconds;
            length.Content = $"Length: {TimeUtils.FormatSecondsToTime((int)Math.Round(lengthValue))}";
        }
    }
}