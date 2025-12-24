using DLClip;
using DLClip.Utils;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;

namespace DLClip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool isVideo;
        bool isLoaded = false;
        bool isAudioOnly;
        private bool _isUpdatingTrimLength = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var app = ((App)Application.Current);

            await app.RefreshToolStatusAsync();
            UiUtils.SetLoadedUiState(false, importBox, trimLengthBox, videoOptionsBox, outputBox, loadButton, loadedLabel, useURLCheckbox, filenameText, runButton);

            if (!app.FfmpegOk || !app.FfprobeOk)
            {
                bool ok = await app.ForceSetupAsync();
                if (!ok)
                {
                    app.Shutdown();
                    return;
                }
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            isAudioOnly = extractAudioCheckbox.IsChecked == true;
        }

        private void chooseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Filter = "Video Files|*.mp4;*.mkv;*.mov;*.webm;*.avi;*.flv;*.gif|Audio Files|*.mp3;*.wav;*.flac;*.m4a;*.ogg;*.opus";
            if (openFile.ShowDialog() == true)
            {
                openFile.CheckFileExists = true;
                openFile.CheckPathExists = true;
                inputText.Text = openFile.FileName;
                inputFormatLabel.Content = Path.GetExtension(inputText.Text);
                if (FormatUtils.IsVideoFormat(Path.GetExtension(inputText.Text))) { isVideo = true; } else { isVideo = false; }
            }
            else
            {
                inputText.Text = "";
            }
        }

        private async void loadButton_Click(object sender, RoutedEventArgs e)
        {
            var app = ((App)Application.Current);

            bool usingURL = (useURLCheckbox.IsChecked == true);
            bool ytdlpOk = app.YtdlpOk;

            if (!isLoaded)
            {
                loadedLabel.Content = "Parsing file info...";
                if (!(await app.ForceSetupAsync())) { loadedLabel.Content = "Not loaded yet..."; return; } // if tools arent ok, fail
                ytdlpOk = app.YtdlpOk; 
                usingURL = (useURLCheckbox.IsChecked == true);

                // validate file path input
                var validationResult = ValidationUtils.ValidateLoadRequest(inputText.Text, usingURL, ytdlpOk);
                if (validationResult.Severity != Models.ValidationSeverity.Success)
                {
                    UiUtils.HandleValidation(validationResult);
                    loadedLabel.Content = "Not loaded yet..."; 
                    return;
                }

                // ffprobe stuff to fill out all the boxes with stuff, yt-dlp to probe url info if using url

                // Loading finishes successfully
                isLoaded = true;
                UiUtils.SetLoadedUiState(true, importBox, trimLengthBox, videoOptionsBox, outputBox, loadButton, loadedLabel, useURLCheckbox, filenameText, runButton);
            }
            else
            {
                // if it is BEING UNLOADED

                loadedLabel.Content = "Unloading...";

                // Unloading finishes successfully

                isLoaded = false;
                UiUtils.SetLoadedUiState(false, importBox, trimLengthBox, videoOptionsBox, outputBox, loadButton, loadedLabel, useURLCheckbox, filenameText, runButton);
            }

        }

        private async void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            var app = ((App)Application.Current);

            SettingsWindow settings = new SettingsWindow();
            settings.ShowDialog();
            await app.RefreshToolStatusAsync();
            if (!app.FfmpegOk || !app.FfprobeOk)
            {
                bool ok = await app.ForceSetupAsync();
                if (!ok)
                {
                    app.Shutdown();
                    return;
                }
            }
            UiUtils.UpdateUrlAvailabilityUi(useURLCheckbox, isLoaded, app.YtdlpOk);
        }

        private void startTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdatingTrimLength) return;
            _isUpdatingTrimLength = true;

            if (startTimeSlider.Value > endTimeSlider.Value)
            {
                endTimeSlider.Value = startTimeSlider.Value;
            }

            startTime.Text = TimeUtils.FormatSecondsToTime((int)Math.Round(startTimeSlider.Value));
            inputLengthLabel.Content = TimeUtils.FormatSecondsToTime((int)Math.Round(endTimeSlider.Value - startTimeSlider.Value));
            _isUpdatingTrimLength = false;
        }

        private void endTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdatingTrimLength) return;
            _isUpdatingTrimLength = true;

            if (endTimeSlider.Value < startTimeSlider.Value)
            {
                startTimeSlider.Value = endTimeSlider.Value;
            }

            endTime.Text = TimeUtils.FormatSecondsToTime((int)Math.Round(endTimeSlider.Value));
            inputLengthLabel.Content = TimeUtils.FormatSecondsToTime((int)Math.Round(endTimeSlider.Value - startTimeSlider.Value));
            _isUpdatingTrimLength = false;
        }

        private void startTime_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingTrimLength) return;
            _isUpdatingTrimLength = true;
            int parsed = TimeUtils.FormatTimeToSeconds(startTime.Text);
            if (parsed == -1)
            {
                startTime.Text = TimeUtils.FormatSecondsToTime((int)Math.Round(startTimeSlider.Value));
                _isUpdatingTrimLength = false;
                return;
            }
            startTimeSlider.Value = parsed;

            if (startTimeSlider.Value > endTimeSlider.Value)
            {
                endTimeSlider.Value = startTimeSlider.Value;
            }

            inputLengthLabel.Content = TimeUtils.FormatSecondsToTime((int)Math.Round(endTimeSlider.Value - startTimeSlider.Value));
            _isUpdatingTrimLength = false;
        }

        private void endTime_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingTrimLength) return;
            _isUpdatingTrimLength = true;
            int parsed = TimeUtils.FormatTimeToSeconds(endTime.Text);
            if (parsed == -1)
            {
                endTime.Text = TimeUtils.FormatSecondsToTime((int)Math.Round(endTimeSlider.Value));
                _isUpdatingTrimLength = false;
                return;
            }
            endTimeSlider.Value = parsed;

            if (endTimeSlider.Value < startTimeSlider.Value)
            {
                startTimeSlider.Value = endTimeSlider.Value;
            }

            inputLengthLabel.Content = TimeUtils.FormatSecondsToTime((int)Math.Round(endTimeSlider.Value - startTimeSlider.Value));
            _isUpdatingTrimLength = false;
        }
    }
}