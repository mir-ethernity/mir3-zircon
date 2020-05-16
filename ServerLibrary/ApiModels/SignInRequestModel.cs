using System;
using System.Collections.Generic;
using System.Text;

namespace Server.ApiModels
{
    public class SignInRequestModel
    {
        public bool AdminArea { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
