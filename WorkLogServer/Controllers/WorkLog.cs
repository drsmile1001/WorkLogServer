using System;

namespace WorkLogServer.Controllers
{
    public class WorkLog
    {
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public string Type { get; set; }

        public string Project { get; set; }

        public string Subject { get; set; }
    }
}