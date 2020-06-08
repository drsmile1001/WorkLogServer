using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Flurl.Http;
using Ical.Net;

namespace WorkLogServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkLogsController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IEnumerable<WorkLog>> Get(string id,string key)
        {
            var cal = await $@"https://calendar.google.com/calendar/ical/{id}%40group.calendar.google.com/private-{key}/basic.ics".GetStringAsync();
            return Calendar.Load(cal).Events.Select(e =>
            {
                var summaryParts = e.Summary.Split('-');

                return new WorkLog
                {
                    StartTime = e.Start.AsDateTimeOffset,
                    EndTime = e.End.AsDateTimeOffset,
                    Type = summaryParts.Length == 3 ? summaryParts[0] : "未知",
                    Project = summaryParts.Length == 3 ? summaryParts[1] : "未知",
                    Subject = summaryParts.Length == 3 ? summaryParts[2] : e.Summary
                };
            });
        }
    }
}
