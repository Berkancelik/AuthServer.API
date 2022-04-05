using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service
{

    // sadece burada kullacağımız için internal olması yeterlidir. 
    internal class DtoMapper : Profile
    {
        public DtoMapper()
        {
            // aşağıdaki işlemde bir dönüştürme uygulanmaktadır. // araştır....
            CreateMap<ProductDto, Product>().ReverseMap();
            CreateMap<UserAppDto, UserApp>().ReverseMap();

        }

    }
}
