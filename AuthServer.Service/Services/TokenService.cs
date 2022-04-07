using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SharedLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOptions _tokenOptions;

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOptions> options)
        {
            _userManager = userManager;
            _tokenOptions = options.Value;
        }


        private string CreateRefreshToken()
        {
            // aşağıda random değer ürettilir. üretilen random değerin bytlerı alınır .
            // Bu bytle lar yukarıdaki number byte aktarılır
            // aşağıda trilyonda bir oluşurulması imkansız bir byte oluşturacaktır
            var numberByte = new Byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);

        }
        // audienc:  bu token hangi api lere istek yapacağına karşılık gelmektedir.
        private IEnumerable<Claim> GetClaim(UserApp userApp, List<String> audience)
        {
            var userList = new List<Claim> { 

                // nameidentifier : id ye karşılık gelmektedir
            new Claim(ClaimTypes.NameIdentifier, userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email, userApp.Email),
                // identity kütüphanesi gelen token içerisindeki tokenları bulamaz. Bu nedenle bizim role diye 
                // belirlememiz gerkmektedir.
                // aşağıdaki gibi bir tanımlama yaptığımızda 
                new Claim(ClaimTypes.Name, userApp.UserName),
                // jason a identity vermek için jti tanımlaması yapmamız gerekmektedir.
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                //new Claim("email") == new Claim(JwtRegisteredClaimNames.Email)

        };
            // token payload olduğunda burada önce bakar

            userList.AddRange(audience.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
    }
        public TokenDto CreateToken(UserApp userApp)
        {
            throw new NotImplementedException();
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
