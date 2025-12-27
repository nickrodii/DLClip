using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using DLClip.Models;
using DLClip.Utils;
using System.IO;
using System.Text.Json;
using System.Globalization;

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
                ProbeResult pr = await CLIUtils.RunProbe(ffprobeExe, new[] { "-v", "error", "-print_format", "json", "-show_format", "-show_streams" }, input, 60000);
                if (pr.ExitCode != 0 || pr.StdOut == null) { return new ProbeMediaResult { Success = false, ErrorTitle = "FFprobe Error", ErrorMessage = pr.StdErr }; }

                ProbeMediaResult mediaInfo = new ProbeMediaResult();
                JsonDocument ffprobeJson = JsonDocument.Parse(pr.StdOut);
                var root = ffprobeJson.RootElement;
                var format = root.GetProperty("format");
                var streams = root.GetProperty("streams");

                string durStr = format.GetProperty("duration").GetString() ?? throw new InvalidOperationException("Duration missing");
                bool parseOk = double.TryParse(durStr, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out double dur);
                if (!parseOk) { return new ProbeMediaResult { Success = false, ErrorTitle = "FFprobe Error", ErrorMessage = "Malformed output" }; }
                mediaInfo.DurationSeconds = (int)Math.Floor(dur);
            }

            // YT-DLP
            else
            {
                ProbeResult pr = await CLIUtils.RunProbe(ytdlpExe, new[] { "--dump-json", "--no-warnings", "--no-playlist" }, input, 60000);
                if (pr.ExitCode != 0)
                {
                    return new ProbeMediaResult { Success = false, ErrorTitle = "yt-dlp Error", ErrorMessage = pr.StdErr };
                }
            }

            return new ProbeMediaResult { Success = false, ErrorTitle = "Not implemented", ErrorMessage = "Media probing is not yet implemented." };
            }
    }
}
