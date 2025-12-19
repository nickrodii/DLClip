using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using Application = System.Windows.Application;

namespace DLClip
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
            ffmpegPathText2.Text = Settings.Default.ffmpegPathText;
            ytdlpPathText2.Text = Settings.Default.ytdlpPathText;
        }

        private void settingsApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // Folder / Binary validation
            if (!Directory.Exists(ffmpegPathText2.Text))
            {
                MessageBox.Show("Directory provided for FFmpeg does not exist. Please specify a valid path to the root folder of FFmpeg's installation.", "Invalid Directory Error");
                DialogResult = false;
                return;
            }
            if (!Directory.Exists(Path.Combine(ffmpegPathText2.Text, "bin")))
            {
                MessageBox.Show("No bin folder found within the FFmpeg installation. Please specify a valid path to the root folder of FFmpeg's installation.", "No Bin Directory Error");
                DialogResult = false;
                return;
            }
            if (!File.Exists(Path.Combine(ffmpegPathText2.Text, "bin", "ffmpeg.exe")))
            {
                MessageBox.Show("No FFmpeg binary found within the bin folder. Reinstall FFmpeg or manually add the binary file to the bin folder.", "No FFmpeg Binary Error");
                DialogResult = false;
                return;
            }
            if (!File.Exists(Path.Combine(ffmpegPathText2.Text, "bin", "ffprobe.exe")))
            {
                DialogResult = false;
                MessageBox.Show("No FFprobe binary found within the bin folder. Make sure to install the full FFmpeg package that includes FFprobe or manually add the binary file to the bin folder.", "No FFprobe Binary Error");
                return;
            }
            Settings.Default.ffmpegPathText = ffmpegPathText2.Text;
            Settings.Default.ffmpegPath = Path.Combine(ffmpegPathText2.Text, "bin");
            Settings.Default.Save();

            if (!Directory.Exists(ytdlpPathText2.Text))
            {
                MessageBox.Show("Directory provided for yt-dlp does not exist. Please specify a valid path to the root folder of yt-dlp's installation.\n\nImporting media from URL will be disabled until yt-dlp is properly installed.", "Invalid Directory Error");
                DialogResult = true;
                this.Close();
                return;
            }
            if (!File.Exists(Path.Combine(ytdlpPathText2.Text, "yt-dlp.exe")))
            {
                MessageBox.Show("No yt-dlp binary found within the bin folder. Reinstall yt-dlp or manually add the binary file to the folder provided.\n\nImporting media from URL will be disabled until yt-dlp is properly installed.", "No yt-dlp Binary Error");
                DialogResult = true;
                this.Close();
                return;
            }

            Settings.Default.ytdlpPathText = ytdlpPathText2.Text;
            Settings.Default.ytdlpPath = ytdlpPathText2.Text;
            Settings.Default.Save();
            DialogResult = true;
            this.Close();
        }

        private void chooseFfmpegButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFolder = new System.Windows.Forms.FolderBrowserDialog();
            if (openFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ffmpegPathText2.Text = openFolder.SelectedPath;
            }
            else
            {
                ffmpegPathText2.Text = "";
            }
        }

        private void settingsCancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void chooseYtdlpButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFolder = new System.Windows.Forms.FolderBrowserDialog();
            if (openFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ytdlpPathText2.Text = openFolder.SelectedPath;
            }
            else
            {
                ytdlpPathText2.Text = "";
            }
        }

        private void ytdlpPathText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

