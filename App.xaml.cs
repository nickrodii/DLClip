using System.Configuration;
using System.Data;
using System.Windows;
using Application = System.Windows.Application;
using System.IO;
using Path = System.IO.Path;

namespace DLClip
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        public bool ytdlpInstalled = false;
        public bool ffmpegInstalled = false;
        protected override void OnStartup(StartupEventArgs e)
        {
            // COMMENT / UNCOMMENT TO RESET SETTINGS
            DLClip.Settings.Default.Reset();
            base.OnStartup(e);
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // update booleans
            ffmpegInstalled = File.Exists(Path.Combine(Settings.Default.ffmpegPath, "bin", "ffmpeg.exe")) || File.Exists(Path.Combine(Settings.Default.ffmpegPath, "bin", "ffprobe.exe"));
            ytdlpInstalled = File.Exists(Path.Combine(Settings.Default.ytdlpPath, "yt-dlp.exe"));

            if (ffmpegInstalled == false)
            {
                SetupWindow setup = new SetupWindow();
                bool? result = setup.ShowDialog();
                if (result == true)
                {
                    OpenMainWindow();
                } else
                {
                    Application.Current.Shutdown();
                }
            } else
            {
                OpenMainWindow();
            }
                
        }

        private void OpenMainWindow()
        {
            MainWindow main = new MainWindow();
            Application.Current.MainWindow = main;
            main.Show();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }

}
