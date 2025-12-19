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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            ffmpegPathText.Text = Settings.Default.ffmpegPath;
            ytdlpPathText.Text = Settings.Default.ytdlpPath;
        }

        

        private async void settingsApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ValidationResult ffmpegValid = await ValidationUtils.ValidateFfmpeg(ffmpegPathText.Text);
            if (!UiUtils.HandleValidation(ffmpegValid))
            {
                DialogResult = false;
                return;
            }
                
            ValidationResult ytdlpValid = await ValidationUtils.ValidateYtdlp(ytdlpPathText.Text);
            UiUtils.HandleValidation(ytdlpValid);
            Settings.Default.ffmpegPath = ffmpegPathText.Text;
            Settings.Default.ytdlpPath = ytdlpPathText.Text;
            Settings.Default.Save();
            DialogResult = true;
            this.Close();
        }

        private void chooseFfmpegButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFolder = new System.Windows.Forms.FolderBrowserDialog();
            if (openFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ffmpegPathText.Text = openFolder.SelectedPath;
            }
            else
            {
                ffmpegPathText.Text = "";
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
                ytdlpPathText.Text = openFolder.SelectedPath;
            }
            else
            {
                ytdlpPathText.Text = "";
            }
        }

        private void ytdlpPathText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
