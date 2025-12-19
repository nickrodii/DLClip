using System.Configuration;
using System.Data;
using System.Windows;
using Application = System.Windows.Application;
using System.IO;
using Path = System.IO.Path;
using DLClip.Utils;

namespace DLClip
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public bool FfmpegOk { get; set; }
        public bool FfprobeOk { get; set; }
        public bool YtdlpOk { get; set; }
        public event EventHandler? ToolStatusChanged;
        protected async override void OnStartup(StartupEventArgs e)
        {
            // COMMENT / UNCOMMENT TO RESET SETTINGS
            // DLClip.Settings.Default.Reset();
            base.OnStartup(e);
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            await RefreshToolStatusAsync();

            if (!FfmpegOk || !FfprobeOk)
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

        public async Task RefreshToolStatusAsync()
        {
            FfmpegOk = await CLIUtils.IsFfmpegOk();
            FfprobeOk = await CLIUtils.IsFfprobeOk();
            YtdlpOk = await CLIUtils.IsYtDlpOk();

            ToolStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task<bool> ForceSetupAsync()
        {
            await RefreshToolStatusAsync();
            if (FfmpegOk && FfprobeOk)
            {
                return true;
            }
            
            SetupWindow setupWindow = new SetupWindow();
            bool? dialogResult = setupWindow.ShowDialog();

            if (dialogResult != true)
            {
                return false;
            }

                await RefreshToolStatusAsync();
            return FfmpegOk && FfprobeOk;
        }
    }
}

