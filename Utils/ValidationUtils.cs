using System;
using System.IO;
using DLClip.Models;
using ValidationResult = DLClip.Models.ValidationResult;
using System.Threading.Tasks;

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
    }
}
