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
        public static TrimDefaults ComputeTrimDefaults(int durationSeconds)
        {
            return new TrimDefaults { DurationSeconds = durationSeconds, StartSeconds = 0, EndSeconds = durationSeconds };
        }

        public static async Task<ProbeMediaResult> ProbeMedia(string input, bool usingURL)
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

                // media duration
                string durStr = format.GetProperty("duration").GetString() ?? throw new InvalidOperationException("Duration missing");
                bool durParseOk = double.TryParse(durStr, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out double dur);
                if (!durParseOk) { return new ProbeMediaResult { Success = false, ErrorTitle = "FFprobe Error", ErrorMessage = "Malformed output" }; }
                mediaInfo.DurationSeconds = (int)Math.Floor(dur);
                // file format
                mediaInfo.ContainerFormat = format.GetProperty("format_name").GetString() ?? throw new InvalidOperationException("Duration missing");


                foreach (JsonElement stream in streams.EnumerateArray())
                {
                    string type = stream.GetProperty("codec_type").GetString() ?? throw new InvalidOperationException("File type missing");

                    // VIDEO STREAM (if there is one)
                    if (type == "video")
                    {
                        mediaInfo.HasVideo = true;
                        // VIDEO
                        if (mediaInfo.HasVideo)
                        {
                            // width
                            mediaInfo.Width = stream.GetProperty("width").GetInt32();
                            // height
                            mediaInfo.Height = stream.GetProperty("height").GetInt32();
                            // codec
                            mediaInfo.VideoCodec = stream.GetProperty("codec_name").GetString() ?? throw new InvalidOperationException("Duration missing");
                            // video bitrate
                            if (stream.TryGetProperty("bit_rate", out var vBr) && int.TryParse(vBr.GetString(), out int vKbps))
                            {
                                mediaInfo.VideoBitrateKbps = vKbps / 1000;
                            }
                            // fps
                            string framerateStr = stream.GetProperty("r_frame_rate").GetString() ?? throw new InvalidOperationException("framerate missing");
                            string[] fpsSplit = framerateStr.Split('/');
                            mediaInfo.Fps = Double.Parse(fpsSplit[0]) / Double.Parse(fpsSplit[1]);
                        }

                    } else if (type == "audio") // AUDIO STREAM
                    {
                        // size estimation
                        // hasAudio and audiobitrate
                        mediaInfo.HasAudio = true;

                        if (stream.TryGetProperty("bit_rate", out var aBr) && int.TryParse(aBr.GetString(), out int aKbps))
                        {
                            mediaInfo.AudioBitrateKbps = aKbps / 1000;
                        }
                    }

                    mediaInfo.Success = true;
                    return mediaInfo;
                }
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
