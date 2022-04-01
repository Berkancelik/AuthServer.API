using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Models
{
    // Bir projeye başlarken mutlaka identity kullanmamız gerekmektedir. İstediğimiz gibi customize edebiliriz.
    public class UserApp:IdentityUser
    {
        public int City { get; set; }
    }
}
