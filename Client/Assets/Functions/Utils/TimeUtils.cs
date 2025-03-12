using System;
using System.Collections.Generic;

namespace Utils
{
    public static class TimeUtils
    {
        public static string GetDateTimeDay(DateTime dateTime, string firstString = "ngày ")
        {
            return $"{firstString}{dateTime:dd/MM/yyyy}";
        }

        public static string GetDateTimeDayTime(DateTime dateTime, string firstString = "ngày ")
        {
            return $"{firstString}{dateTime:dd/MM/yyyy HH:mm}";
        }
        public static string GetTimeDifference(DateTime from, DateTime to)
        {
            TimeSpan timeSpan = to - from;
            int years = (int)(timeSpan.TotalDays / 365);
            int months = (int)((timeSpan.TotalDays % 365) / 30);
            int days = (int)(timeSpan.TotalDays % 30);
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;

            List<string> parts = new List<string>();
            if (years > 0) parts.Add($"{years} năm");
            if (months > 0) parts.Add($"{months} tháng");
            if (days > 0) parts.Add($"{days} ngày");
            if (hours > 0) parts.Add($"{hours} giờ");
            if (minutes > 0) parts.Add($"{minutes} phút");

            return string.Join(", ", parts);
        }
        public static string GetTimeRemaining(DateTime dateTime)
        {
            TimeSpan remainingTime = dateTime - DateTime.Now;

            if (remainingTime.TotalSeconds <= 0)
            {
                return "0 giây";
            }

            if (remainingTime.Days > 0)
            {
                return $"{remainingTime.Days} ngày";
            }
            else if (remainingTime.Hours > 0)
            {
                return $"{remainingTime.Hours} giờ";
            }
            else if (remainingTime.Minutes > 0)
            {
                return $"{remainingTime.Minutes} phút";
            }
            else
            {
                return $"{remainingTime.Seconds} giây";
            }
        }
    }
}