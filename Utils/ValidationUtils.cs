using System;
using System.IO;
using DLClip.Models;
using ValidationResult = DLClip.Models.ValidationResult;
using System.Threading.Tasks;
using DLClip;
using DLClip.Utils;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;

namespace DLClip.Utils
{
    internal static class ValidationUtils
    {
        public async static Task<ValidationResult> ValidateFfmpeg(string rootPath)
        {
            // ERROR: NO FOLDER SPECIFIED
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                return ValidationResult.Failure("No folder specified for FFmpeg's installation. Please specify a valid path to the root folder of FFmpeg's installation.", "Error: No folder specified");
            }

            // ERROR: FOLDER DOESN'T EXIST
            if (!Directory.Exists(rootPath))
            {
                return ValidationResult.Failure("Folder specified for FFmpeg's installation does not exist. Please specify a valid path to the root folder of FFmpeg's installation.", "Error: Folder doesn't exist");
            }

            // ERROR: BIN SUBFOLDER DOESN'T EXIST
            if (!Directory.Exists(Path.Combine(rootPath, "bin")))
            {
                return ValidationResult.Failure("Bin subfolder for FFmpeg's installation was not found. Please specify a valid path to the root folder of an unmodified FFmpeg installation.", "Error: Bin subfolder doesn't exist");
            }

            // ERROR: EXECUTABLE DOESN'T EXIST (ffmpeg.exe)
            if (!File.Exists(Path.Combine(rootPath, "bin", "ffmpeg.exe")))
            {
                return ValidationResult.Failure("ffmpeg.exe was not found. Please specify a valid path to the root folder of an unmodified FFmpeg installation.", "Error: Executable doesn't exist");
            }

            // ERROR: EXECUTABLE DOESN'T EXIST (ffprobe.exe)
            if (!File.Exists(Path.Combine(rootPath, "bin", "ffprobe.exe")))
            {
                return ValidationResult.Failure("ffprobe.exe was not found. Please specify a valid path to the root folder of a full FFmpeg installation that includes ffprobe.", "Error: Executable doesn't exist");
            }

            // ERROR: EXECUTABLE DOESN'T RUN PROPERLY (ffmpeg.exe)
            bool ffmpegOk = await CLIUtils.IsFfmpegOk(rootPath);
            if (!ffmpegOk)
            {
                return ValidationResult.Failure("FFmpeg was found, but is not running properly. Please reinstall FFmpeg and provide a valid path to the root folder of that installation.", "Error: Executable doesn't run properly");
            }

            // ERROR: EXECUTABLE DOESN'T RUN PROPERLY (ffprobe.exe)
            bool ffprobeOk = await CLIUtils.IsFfprobeOk(rootPath);
            if (!ffprobeOk)
            {
                return ValidationResult.Failure("FFmpeg and FFprobe were found, but FFprobe is not running properly. Please reinstall FFmpeg and provide a valid path to the root folder of that installation.", "Error: Executable doesn't run properly");
            }

            return ValidationResult.Success();
        }

        public async static Task<ValidationResult> ValidateYtdlp(string rootPath)
        {
            const string disabledUrlNotice = "\n\nImporting from URL will be disabled until a valid root folder of yt-dlp's installation is provided.";
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                return ValidationResult.Warning("No folder specified for yt-dlp's installation." + disabledUrlNotice, "Warning: No folder specified");
            }

            if (!Directory.Exists(rootPath))
            {
                return ValidationResult.Warning("Folder specified for yt-dlp's installation does not exist. Please specify a valid path to the root folder of yt-dlp's installation." + disabledUrlNotice, "Warning: Folder doesn't exist");
            }

            if (!File.Exists(Path.Combine(rootPath, "yt-dlp.exe")))
            {
                return ValidationResult.Warning("yt-dlp.exe was not found. Please specify a valid path to the root folder of an unmodified yt-dlp installation." + disabledUrlNotice, "Warning: Executable doesn't exist");
            }

            bool ytdlpOk = await CLIUtils.IsYtDlpOk(rootPath);
            if (!ytdlpOk)
            {
                return ValidationResult.Warning("yt-dlp was found, but is not running properly. Please reinstall yt-dlp and provide a valid path to the root folder of that installation." + disabledUrlNotice, "Warning: Executable doesn't run properly");
            }

            return ValidationResult.Success();

        }

        public static ValidationResult ValidateLoadRequest(string inputPath, bool usingURL, bool ytdlpOk)
        {
            // LOCAL FILE MODE
            if (!usingURL)
            {
                if (string.IsNullOrWhiteSpace(inputPath) || !File.Exists(inputPath))
                {
                    return ValidationResult.Failure("Please enter a valid video or audio file path. Select \"Choose File...\" to find one.", "Error: File path doesn't exist");
                }

                if (!FormatUtils.IsValidFormat(Path.GetExtension(inputPath)))
                {
                    return ValidationResult.Failure("Please use a valid video or audio format. Formats include: .mp4, .mkv, .mov, .webm, .avi, .flv, .gif, .mp3, .wav, .flac, .m4a, .ogg, .opus", "Error: Invalid media format");
                }

                return ValidationResult.Success();
            }

            // URL MODE
            else
            {
                if (!ytdlpOk) { return ValidationResult.Failure("yt-dlp is not configured properly. Please configure the yt-dlp installation path to be able to import from URL.", "Error: yt-dlp not configured"); }

                if (string.IsNullOrWhiteSpace(inputPath))
                {
                    return ValidationResult.Failure("Please enter a valid media URL.", "Error: URL not specified");
                }

                return ValidationResult.Success();
            }
        }
    }

}
