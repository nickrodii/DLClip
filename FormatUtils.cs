namespace DLClip
{
    class FormatUtils
    {
        private static HashSet<String> videoFormats = new HashSet<String> { ".mp4", ".mkv", ".mov", ".webm", ".avi", ".flv", ".gif" };
        private static HashSet<String> audioFormats = new HashSet<String> { ".mp3", ".wav", ".flac", ".m4a", ".ogg", ".opus" };

        public static bool IsVideoFormat(String format)
        {
            return videoFormats.Contains(format.ToLower());
        }

        public static bool IsValidFormat(String format)
        {
            if (videoFormats.Contains(format.ToLower()) || audioFormats.Contains(format.ToLower())) return true;
            return false;
        }
    }
}
