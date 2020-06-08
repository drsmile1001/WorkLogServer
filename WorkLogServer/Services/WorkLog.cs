using System;

namespace WorkLogServer.Services
{
    public class WorkLog
    {
        public string? Person { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public string Type { get; set; } = "";

        public string Project { get; set; } = "";

        public string Subject { get; set; } = "";

        public double Duration => (EndTime - StartTime).TotalHours;

        public DateTime Date => StartTime.LocalDateTime.Date;
    }
}