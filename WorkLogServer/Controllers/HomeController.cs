using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorkLogServer.Services;

namespace WorkLogServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly WorkLogService _workLogService;

        public HomeController(WorkLogService workLogService)
        {
            _workLogService = workLogService;
        }

        public async Task<IActionResult> Index(string id,DateTime? start = null,DateTime? end = null)
        {
            var entries = await _workLogService.GetCalendarEntries(id);

            var allPersonWorkLogs = new List<WorkLog>();

            foreach (var entry in entries)
            {
                var workLogs = await _workLogService.Get(entry.Id, entry.Key, entry.Person);
                allPersonWorkLogs.AddRange(workLogs);
            }
            var model = allPersonWorkLogs
                .Where(log => log.InRange(start, end))
                .OrderBy(log=>log.Person)
                .ThenByDescending(log=>log.StartTime)
                .ToArray();
            return View(model);
        }
    }
}