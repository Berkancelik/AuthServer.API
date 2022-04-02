using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class ErrorDto
    {
        // sadece bu class içinde set edilebilir set=private
        // hata listesi string olarak tutulmaktadır.
        public List<String>Errors { get; private set; }
        // gelen hatayı biz IsShow da true olarak gösterirsek bu hata kullanıcıya gösterilir
        public bool IsShow { get; private set; }

        public ErrorDto()
        {
            Errors = new List<string>();
        }

        public ErrorDto(string error, bool isShow)
        {
            Errors.Add(error);
            isShow = true;
        }

        public ErrorDto(List<string> errors,bool isShow)
        {
            Errors = errors;
            IsShow = isShow;
            
        }
    }
}
