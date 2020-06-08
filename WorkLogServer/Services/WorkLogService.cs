using Flurl.Http;
using Ical.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WorkLogServer.Services
{
    public class WorkLogService
    {
        public async Task<WorkLog[]> Get(string id, string key)
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
            }).ToArray();
        }

        public async Task<CalendarEntry[]> GetCalendarEntries(string id)
        {
            var json = await $"https://spreadsheets.google.com/feeds/cells/{id}/1/public/values?alt=json".GetStringAsync();
            using var jsonDocument = JsonDocument.Parse(json);
            return jsonDocument.RootElement
                .GetProperty("feed")
                .GetProperty("entry")
                .EnumerateArray()
                .Select(entry => entry.GetProperty("gs$cell"))
                .Select(cell => new
                {
                    Row = cell.GetProperty("row").GetString(),
                    Col = cell.GetProperty("col").GetString(),
                    Value = cell.GetProperty("$t").GetString()
                })
                .Where(cell => cell.Row != "1")
                .GroupBy(cell => cell.Row)
                .Where(rowGroup => rowGroup.Count() == 3)
                .Select(rowGroup => new CalendarEntry
                {
                    Person = rowGroup.Single(cell => cell.Col == "1").Value,
                    Id = rowGroup.Single(cell => cell.Col == "2").Value,
                    Key = rowGroup.Single(cell => cell.Col == "3").Value,
                }).ToArray();
        }
    }
}
