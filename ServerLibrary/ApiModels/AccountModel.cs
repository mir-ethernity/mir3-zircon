using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Server.ApiModels
{
    public class AccountModel
    {
        public string Email { get; set; }
        public string RealName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
