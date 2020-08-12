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

        [Route("Home/Index")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Route("Home/Search")]
        [HttpGet]
        public ActionResult Search(string id,DateTime? start = null,DateTime? end = null)
        {
            return RedirectToAction(nameof(Print), new
            {
                id,
                start,
                end
            });
        }

        [Route("Home/Index/{id}")]
        [HttpGet]
        public async Task<IActionResult> Print(string id,DateTime? start = null,DateTime? end = null)
        {
            CalendarEntry[]? entries;
            try
            {
                entries = await _workLogService.GetCalendarEntries(id);
            }
            catch (Exception)
            {
                return View("Index",new Dictionary<string,string>
                {
                    ["error"] = "讀取行事曆註冊表發生錯誤"
                });
            }

            var getWorkLogTasks = entries!.Select(entry => 
            {
                return Task.Run(async () =>
                {
                    try
                    {
                        return (logs: await _workLogService.Get(entry.Id, entry.Key, entry.Person),error:(string?)null);
                    }
                    catch (Exception)
                    {
                        return (logs: (WorkLog[]?)null, error: $"無法讀取{entry.Person}的行事曆");
                    }
                });
            });

            var taskResults = await Task.WhenAll(getWorkLogTasks);
            var errors = taskResults.Where(item => item.error != null)
                .Select(item => item.error!)
                .ToArray();

            if (errors.Any())
            {
                return View("Index", new Dictionary<string, string>
                {
                    ["error"] = string.Join('|', errors)
                });
            }

            var model = taskResults
                .Select(item=>item.logs)
                .SelectMany(log=>log)
                .Where(log => log.InRange(start, end))
                .OrderBy(log=>log.Person)
                .ThenByDescending(log=>log.StartTime)
                .ToArray();
            return View(model);
        }
    }
}