using DLClip.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DLClip.Utils
{
    internal class CLIUtils
    {
        
       
        
        public static async Task<bool> IsFfmpegOk()
        {
            string ffmpegExe = Path.Combine(Settings.Default.ffmpegPathText, "bin", "ffmpeg.exe");

            if (!Directory.Exists(Settings.Default.ffmpegPathText) || !File.Exists(ffmpegExe))
            {
                return false;
            }
            
            ProbeResult result = await RunProbe(ffmpegExe, "-version", 2000);
            if (result.TimedOut == false && result.ExitCode == 0 && result.StdOut.Contains("ffmpeg version"))
            {
                return true;
            } else
            {
                return false;
            }
        }

        public static async Task<bool> IsFfprobeOk()
        {
            string ffprobeExe = Path.Combine(Settings.Default.ffmpegPathText, "bin", "ffprobe.exe");

            if (!File.Exists(ffprobeExe))
            {
                return false;
            }

            ProbeResult result = await RunProbe(ffprobeExe, "-version", 2000);
            if (result.TimedOut == false && result.ExitCode == 0 && result.StdOut.Contains("ffprobe version"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> IsYtDlpOk()
        {
            string ytdlpExe = Path.Combine(Settings.Default.ytdlpPathText, "yt-dlp.exe");

            if (!Directory.Exists(Settings.Default.ytdlpPathText) || !File.Exists(ytdlpExe))
            {
                return false;
            }

            ProbeResult result = await RunProbe(ytdlpExe, "--version", 3000);
            if (result.TimedOut == false && result.ExitCode == 0 && !String.IsNullOrEmpty(result.StdOut?.Trim()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async static Task<ProbeResult> RunProbe(string exePath, string args, int timeout)
        {
            ProbeResult result = new ProbeResult();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            if (!File.Exists(exePath))
            {
                result.ErrorMessage = "File at exe path doesn't exist";
                return result;
            }

            Process process = new Process();
            process.StartInfo = startInfo;
            try
            {
                process.Start();
                Task<String> stdOutTask = process.StandardOutput.ReadToEndAsync();
                Task<String> stdErrTask = process.StandardError.ReadToEndAsync();

                Task exitTask = process.WaitForExitAsync();
                Task timeoutTask = Task.Delay(timeout);

                Task completedTask = await Task.WhenAny(exitTask, timeoutTask);
                if (completedTask == timeoutTask)
                {
                    result.TimedOut = true;
                    result.ErrorMessage = "Process timed out";
                    process.Kill(entireProcessTree: true);
                    await exitTask;
                    result.StdOut = await stdOutTask;
                    result.StdErr = await stdErrTask;
                    return result;
                }
                else
                {
                    await process.WaitForExitAsync();
                    result.ExitCode = process.ExitCode;
                    result.StdOut = await stdOutTask;
                    result.StdErr = await stdErrTask;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                return result;
            }
            finally
            {
                process.Dispose();
            }
        }
    }
}
