using System;
using System.Collections.Generic;
using System.Text;

namespace DLClip.Utils
{
    internal class TimeUtils
    {
        public static string FormatSecondsToTime(int totalSeconds)
        {
            int remainingSeconds = totalSeconds;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            string hoursStr = "";
            string minutesStr = "";
            string secondsStr = "";

            if (totalSeconds < 0) 
            {
                return "00:00:00";
            }
            if (totalSeconds >= 3600)
            {
                hours = remainingSeconds / 3600;
                remainingSeconds = totalSeconds % 3600;
            }

            if (remainingSeconds >= 60)
            {
                minutes = remainingSeconds / 60;
                remainingSeconds = remainingSeconds % 60;
            }

            seconds = remainingSeconds;

            hoursStr = hours.ToString("D2");
            minutesStr = minutes.ToString("D2");
            secondsStr = seconds.ToString("D2");

            return $"{hoursStr}:{minutesStr}:{secondsStr}";
        }

        public static int FormatTimeToSeconds(string timeStr)
        {
            string[] timeParts = timeStr.Split(':');
            
            if (string.IsNullOrWhiteSpace(timeStr) || timeParts.Length != 3)
            {
                return -1;
            }

            for (int i = 0; i < timeParts.Length; i++)
            {
                if (timeParts[i].Length != 2)
                {
                    return -1;
                }

                if (!int.TryParse(timeParts[i], out int tempNum))
                {
                    return -1;
                }
            }

            int hours = int.Parse(timeParts[0]);
            int minutes = int.Parse(timeParts[1]);
            int seconds = int.Parse(timeParts[2]);

            if (minutes > 59 || seconds > 59 || hours < 0 || minutes < 0 || seconds < 0)
            {
                return -1;
            }

            return (hours * 3600) + (minutes * 60) + seconds;
        }
    }
}
