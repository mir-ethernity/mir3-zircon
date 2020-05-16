using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.ApiModels;
using Server.Envir;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [AllowAnonymous]
        [HttpPost("sign-in")]
        public IActionResult SignIn([FromBody]SignInModel model)
        {
            var account = _accountService.SignIn(model);

            if (account == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(account);
        }
    }
}
