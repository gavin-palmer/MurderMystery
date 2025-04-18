using System;
namespace MurderMystery.Helpers
{
    using System;
    using System.Collections.Generic;

    namespace MurderMystery.Utilities
    {
        public static class TimeFrames
        {
            public static List<string> GetTimeFrames(string startTime, string endTime, int intervalMinutes)
            {
                DateTime start = ParseTime(startTime);
                DateTime end = ParseTime(endTime);

                var timeFrames = new List<string>();
                DateTime current = start;

                while (current <= end)
                {
                    timeFrames.Add(FormatTime(current));
                    current = current.AddMinutes(intervalMinutes);
                }

                return timeFrames;
            }

            private static DateTime ParseTime(string timeStr)
            {
                DateTime baseDate = DateTime.Today;

                if (DateTime.TryParse(timeStr, out DateTime result))
                {
                    return baseDate.Add(result.TimeOfDay);
                }

                if (timeStr.EndsWith("am", StringComparison.OrdinalIgnoreCase) ||
                    timeStr.EndsWith("pm", StringComparison.OrdinalIgnoreCase))
                {
                    string amPm = timeStr.Substring(timeStr.Length - 2);
                    string hourPart = timeStr.Substring(0, timeStr.Length - 2);

                    if (int.TryParse(hourPart, out int hour))
                    {
                        if (amPm.Equals("pm", StringComparison.OrdinalIgnoreCase) && hour < 12)
                            hour += 12;
                        else if (amPm.Equals("am", StringComparison.OrdinalIgnoreCase) && hour == 12)
                            hour = 0;

                        return baseDate.AddHours(hour);
                    }
                }

                throw new ArgumentException($"Unable to parse time string: {timeStr}");
            }

            private static string FormatTime(DateTime time)
            {
                return time.ToString("h:mmtt").ToLower();
            }
        }
    }
}
