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

        public MainWindow()
        {
            InitializeComponent();
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var app = ((App)Application.Current);

            await app.RefreshToolStatusAsync();
            UiUtils.UpdateUrlAvailabilityUi(useURLCheckbox, isLoaded, app.YtdlpOk);

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
            bool usingURL = (useURLCheckbox.IsChecked == true);
            var app = ((App)Application.Current);

            if (!isLoaded)
            {
                loadedLabel.Content = "Parsing file info...";

                bool toolsOk = await app.ForceSetupAsync();
                usingURL = useURLCheckbox.IsChecked == true;

                if (!toolsOk) { MessageBox.Show("The CLI tools are not working properly. Please reinstall FFmpeg.", "CLI Tools Error"); return; }
                // Loading process begins

                if (!usingURL)
                {
                    if (string.IsNullOrWhiteSpace(inputText.Text) || !File.Exists(inputText.Text))
                    {
                        loadedLabel.Content = "Not loaded yet...";
                        MessageBox.Show("Please enter a valid video or audio file path. Select \"Choose File...\" to find one.", "File Path Error");
                        return;
                    }

                    if (!FormatUtils.IsValidFormat(Path.GetExtension(inputText.Text)))
                    {
                        loadedLabel.Content = "Not loaded yet...";
                        MessageBox.Show("Please use a valid video or audio format. Formats include: .mp4, .mkv, .mov, .webm, .avi, .flv, .gif, .mp3, .wav, .flac, .m4a, .ogg, .opus", "File Path Error");
                        return;
                    }
                }
                else
                {
                    if (!app.YtdlpOk)
                    {
                        MessageBox.Show("yt-dlp is not working properly. Please reinstall yt-dlp to import from URL.", "Error: Executable not running properly");
                        UiUtils.UpdateUrlAvailabilityUi(useURLCheckbox, false, false);
                        return;
                    }
                }

                // Loading finishes successfully

                inputText.IsEnabled = false;
                chooseFileButton.IsEnabled = false;
                UiUtils.UpdateUrlAvailabilityUi(useURLCheckbox, true, app.YtdlpOk);
                extractAudioCheckbox.IsEnabled = false;
                loadButton.Content = "Unload Media";

                startTime.IsEnabled = true;
                startTimeSlider.IsEnabled = true;
                endTime.IsEnabled = true;
                endTimeSlider.IsEnabled = true;

                resolutionSelections.IsEnabled = true;
                FPSSelections.IsEnabled = true;
                bitrateSelections.IsEnabled = true;

                formatSelections.IsEnabled = true;
                codecSelections.IsEnabled = true;
                filenameText.IsEnabled = true;
                runButton.IsEnabled = true;

                isLoaded = true;
                loadedLabel.Content = "Successfully loaded!";
            }
            else
            {
                // if it is BEING UNLOADED

                loadedLabel.Content = "Unloading...";

                // Unoading finishes successfully

                inputText.IsEnabled = true;
                chooseFileButton.IsEnabled = true;
                await app.RefreshToolStatusAsync();
                UiUtils.UpdateUrlAvailabilityUi(useURLCheckbox, false, app.YtdlpOk);
                extractAudioCheckbox.IsEnabled = true;
                loadButton.Content = "Load Media";

                filenameText.Text = "";

                startTime.IsEnabled = false;
                startTimeSlider.IsEnabled = false;
                endTime.IsEnabled = false;
                endTimeSlider.IsEnabled = false;

                resolutionSelections.IsEnabled = false;
                FPSSelections.IsEnabled = false;
                bitrateSelections.IsEnabled = false;

                formatSelections.IsEnabled = false;
                codecSelections.IsEnabled = false;
                filenameText.IsEnabled = false;
                runButton.IsEnabled = false;

                isLoaded = false;
                loadedLabel.Content = "Not loaded yet...";
            }

        }

        private async void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            var app = ((App)Application.Current);

            SettingsWindow settings = new SettingsWindow();
            settings.ShowDialog();
            if (settings.DialogResult == true)
            {
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
        }
    }
}