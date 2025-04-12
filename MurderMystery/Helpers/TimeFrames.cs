using System;
namespace MurderMystery.Helpers
{
    using System;
    using System.Collections.Generic;

    namespace MurderMystery.Utilities
    {
        public static class TimeFrames
        {
            /// <summary>
            /// Generates a list of time strings at regular intervals
            /// </summary>
            /// <param name="startTime">The starting time (e.g., "6:00pm")</param>
            /// <param name="endTime">The ending time (e.g., "9:00pm")</param>
            /// <param name="intervalMinutes">The interval between times in minutes</param>
            /// <returns>A list of formatted time strings</returns>
            public static List<string> GetTimeFrames(string startTime, string endTime, int intervalMinutes)
            {
                // Parse the input times
                DateTime start = ParseTime(startTime);
                DateTime end = ParseTime(endTime);

                // Generate the list of times
                var timeFrames = new List<string>();
                DateTime current = start;

                while (current <= end)
                {
                    timeFrames.Add(FormatTime(current));
                    current = current.AddMinutes(intervalMinutes);
                }

                return timeFrames;
            }


            /// <summary>
            /// Parses a time string like "6:00pm" into a DateTime object
            /// </summary>
            private static DateTime ParseTime(string timeStr)
            {
                // Base date doesn't matter since we only care about the time portion
                DateTime baseDate = DateTime.Today;

                // Handle various input formats
                if (DateTime.TryParse(timeStr, out DateTime result))
                {
                    // Keep only the time portion
                    return baseDate.Add(result.TimeOfDay);
                }

                // Try to handle abbreviated formats like "6pm"
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

            /// <summary>
            /// Formats a DateTime into a time string like "6:00pm"
            /// </summary>
            private static string FormatTime(DateTime time)
            {
                return time.ToString("h:mmtt").ToLower();
            }
        }
    }
}
