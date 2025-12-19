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
        
       
        
        public static async Task<bool> IsFfmpegOk(string rootPath)
        {
            string ffmpegExe = Path.Combine(rootPath, "bin", "ffmpeg.exe");

            if (!File.Exists(ffmpegExe))
            {
                return false;
            }
            
            ProbeResult result = await RunProbe(ffmpegExe, "-version", 2000);
            return result.TimedOut == false && result.ExitCode == 0 && result.StdOut.Contains("ffmpeg version");
        }

        public static async Task<bool> IsFfprobeOk(string rootPath)
        {
            string ffprobeExe = Path.Combine(rootPath, "bin", "ffprobe.exe");

            if (!File.Exists(ffprobeExe))
            {
                return false;
            }

            ProbeResult result = await RunProbe(ffprobeExe, "-version", 2000);
            return result.TimedOut == false && result.ExitCode == 0 && result.StdOut.Contains("ffprobe version");
        }

        public static async Task<bool> IsYtDlpOk(string rootPath)
        {
            string ytdlpExe = Path.Combine(rootPath, "yt-dlp.exe");

            if (!File.Exists(ytdlpExe))
            {
                return false;
            }

            ProbeResult result = await RunProbe(ytdlpExe, "--version", 3000);
            return result.TimedOut == false && result.ExitCode == 0 && !String.IsNullOrEmpty(result.StdOut?.Trim());
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
