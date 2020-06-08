using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Flurl.Http;
using Ical.Net;
using WorkLogServer.Services;

namespace WorkLogServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkLogsController : ControllerBase
    {
        private readonly WorkLogService _workLogService;

        public WorkLogsController(WorkLogService workLogService)
        {
            _workLogService = workLogService;
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<WorkLog>> Get(string id,string key)
        {
            return await _workLogService.Get(id, key);
        }
    }
}
