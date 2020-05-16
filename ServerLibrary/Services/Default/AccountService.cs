using Microsoft.IdentityModel.Tokens;
using Server.ApiModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Server.Services.Default
{
    public class AccountService : IAccountService
    {
        private SigningCredentials _credentials;

        public AccountService()
        {
            var key = Encoding.ASCII.GetBytes(Config.ApiServerJWTSecret);
            _credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        }

        public SignInResponseModel SignIn(SignInRequestModel model)
        {
            var account = SEnvir.AccountInfoList.Binding.FirstOrDefault(x => x.EMailAddress.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase));

            if (account == null)
                return null;
            else if (!account.Activated)
                return null;
            else if (account.Banned)
                return null;
            else if (model.AdminArea && !account.Admin)
                return null;
            else if (!SEnvir.PasswordMatch(model.Password, account.Password))
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, account.EMailAddress),
                    new Claim(ClaimTypes.Name, account.RealName),
                    new Claim(ClaimTypes.Role, account.Admin ? "admin" : "player")
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = _credentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new SignInResponseModel
            {
                AccessToken = tokenHandler.WriteToken(token),
                Account = new AccountModel
                {
                    Email = account.EMailAddress,
                    RealName = account.RealName,
                    IsAdmin = account.Admin
                }
            };
        }
    }
}
