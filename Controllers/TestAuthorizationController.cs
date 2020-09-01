using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mmt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestAuthorizationController : ControllerBase
    {
        [HttpGet("Guest")]
        [AllowAnonymous]
        public ActionResult OnlyGuest()
        {
            return Ok("Dag Guest!");
        }

        [Authorize("User")]
        [HttpGet("User")]
        public  ActionResult OnlyUsers()
        {
            return Ok("Dag User!");
        }

        [HttpGet("Admin")]
        [Authorize("Admin")]
        public  ActionResult OnlyAdmin()
        {
            return Ok("Dag Admin!");
        }

        [Authorize("Supervisor")]
        [HttpGet("Supervisor")]
        public ActionResult OnlySupervisor()
        {
            return Ok("Dag Supervisor!");
        }

        [Authorize("Director")]
        [HttpGet("Director")]
        public  ActionResult OnlyDirector()
        {
            return Ok("Dag Director!");
        }
    }
}
