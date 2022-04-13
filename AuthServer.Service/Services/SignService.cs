using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    // asimetrik imzalama birbirine dosya göndermek istediğinde private&public keyler vardır
    // A                        B 
    //günümüzdeki haberleşmeler bu yolla yapılmaktadır =>> ssh
    public static class SignService
    {
        public static SecurityKey GetSymmetricSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }
    }
}
