using AuthServer.Core.DTOs;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{

    // identitiy kütüphanesi ile birlikte  hazır metotlar gelemketedir. O sebeple repository kurmaya gerek kalmamaktadır.
    // identity üzerinden 3 adet class hazır gelmektedir: bunlar roleManager, userManager, signInManager. Sadece servisi yazmamız yeterli olur
    public interface IUserService
    {
        Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Response<UserAppDto>> GetUserByNameAsync(string userName);
    }
}
