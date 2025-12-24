using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using DLClip.Models;
using DLClip.Utils;
using System.IO;

namespace DLClip.Services
{
    internal class MediaProbeService
    {
        TrimDefaults ComputeTrimDefaults(int durationSeconds)
        {
            return new TrimDefaults { DurationSeconds = durationSeconds, StartSeconds = 0, EndSeconds = durationSeconds };
        }

        public async Task<ProbeMediaResult> ProbeMedia(string input, bool usingURL)
        {
            string ffprobeExe = Path.Combine(Settings.Default.ffmpegPath, "bin", "ffprobe.exe");
            string ytdlpExe = Path.Combine(Settings.Default.ytdlpPath, "yt-dlp.exe");

            // FFPROBE
            if (!usingURL)
            {
                ProbeResult pr = await CLIUtils.RunProbe(ffprobeExe, new[] { "-v error", "-print_format json", "-show_format", "-show_streams" }, input, 60000);

            }

            // YT-DLP
            else
            {
                await CLIUtils.RunProbe(ytdlpExe, new[] { "--dump-json", "--no-warnings", "--no-playlist" }, input, 60000);
            }

            return new ProbeMediaResult { Success = false, ErrorTitle = "Not implemented", ErrorMessage = "Media probing is not yet implemented." };
            }
    }
}
