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

        public async Task<IActionResult> Index(string id)
        {
            var entries = await _workLogService.GetCalendarEntries(id);

            var allPersonWorkLogs = new List<WorkLog>();

            foreach (var entry in entries)
            {
                var workLogs = await _workLogService.Get(entry.Id, entry.Key);
                foreach (var workLog in workLogs)
                {
                    workLog.Person = entry.Person;
                }
                allPersonWorkLogs.AddRange(workLogs);
            }
            return View(allPersonWorkLogs.ToArray());
        }
    }
}