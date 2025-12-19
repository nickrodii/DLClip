using Microsoft.Win32;
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
using Path = System.IO.Path;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;
using DLClip.Models;
using ValidationResult = DLClip.Models.ValidationResult;
using DLClip.Utils;

namespace DLClip
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// 
    /// Very similar to SettingsWindow, but has extra text to welcome the user
    /// </summary>
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
            ffmpegPathText2.Text = Settings.Default.ffmpegPath;
            ytdlpPathText2.Text = Settings.Default.ytdlpPath;
        }

        private async void settingsApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ValidationResult ffmpegValid = await ValidationUtils.ValidateFfmpeg(ffmpegPathText2.Text);
            if (!UiUtils.HandleValidation(ffmpegValid))
            {
                DialogResult = false;
                return;
            }

            ValidationResult ytdlpValid = await ValidationUtils.ValidateYtdlp(ytdlpPathText2.Text);
            UiUtils.HandleValidation(ytdlpValid);
            Settings.Default.ffmpegPath = ffmpegPathText2.Text;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.OpenUrl("https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip");
        }

        private void ytdlpDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.OpenUrl("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe");
        }
    }
}

