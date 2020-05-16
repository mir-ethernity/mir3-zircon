using System;
using System.Collections.Generic;
using System.Text;

namespace Server.ApiModels
{
    public class SignInResponseModel
    {
        public string AccessToken { get; set; }
        public AccountModel Account { get; set; }
    }
}
