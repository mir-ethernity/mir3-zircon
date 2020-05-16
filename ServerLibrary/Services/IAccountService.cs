using Server.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Services
{
    public interface IAccountService
    {
        SignInResponseModel SignIn(SignInRequestModel model);
    }
}
