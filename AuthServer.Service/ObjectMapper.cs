using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service
{
    public static class ObjectMapper
    {
        // LazyLoading: Sadeced ihtiyaç olduğu anda yüklemesini sağlamaktadır. 
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoMapper>();
            });
            return config.CreateMapper();

        });


        // aşağıdakinin düzeltilmesi gerekebili
        public static IMapper Mapper => lazy.Value;

    }

}
