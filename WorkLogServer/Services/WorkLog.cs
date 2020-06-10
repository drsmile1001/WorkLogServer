using System;

namespace WorkLogServer.Services
{
    public class WorkLog
    {
        private readonly DateTime _startTime;

        private readonly DateTime _endTime;

        public WorkLog(DateTime startTime,DateTime endTime,string summary,string? person = "")
        {
            _startTime = startTime;
            _endTime = endTime;
            Person = person;
            var summaryParts = summary.Split('-');
            Type = summaryParts.Length == 3 ? summaryParts[0] : "未知";
            Project = summaryParts.Length == 3 ? summaryParts[1] : "未知";
            Subject = summaryParts.Length == 3 ? summaryParts[2] : summary;
        }

        public string Type { get; private set; }

        public string Project { get; private set; }

        public string Subject { get; private set; }

        public string StartTime => _startTime.ToString("yyyy-MM-dd HH:mm");

        public string EndTime => _endTime.ToString("yyyy-MM-dd HH:mm");

        public double Duration => (_endTime - _startTime).TotalHours;

        public string Year => _startTime.Date.ToString("yyyy");

        public string Month => _startTime.Date.ToString("yyyy-MM");

        public string Date => _startTime.Date.ToString("yyyy-MM-dd");

        public string? Person { get; private set; }

        public bool InRange(DateTime? start, DateTime? end)
        {
            return (!start.HasValue || _startTime.Date >= start.Value.Date) &&
                (!end.HasValue || _startTime.Date <= end.Value.Date);
        }
    }
}