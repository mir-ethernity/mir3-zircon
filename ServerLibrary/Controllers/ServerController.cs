using Microsoft.AspNetCore.Mvc;
using Server.Envir;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new ServerStatus
            {
                Online = SEnvir.Started,
                Players = SEnvir.Players.Count
            });
        }
    }
}
