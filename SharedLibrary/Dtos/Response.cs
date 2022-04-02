using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class Response<T> where T:class
    {
        public T Data { get; private set; }

        // burada statusCode vermemizin amacı; eğer client ya da proje farklı bir uygualamaya taşındığında error codları da oraya gitsin analamını taşır
        public int StatusCode { get; private set; }

        public ErrorDto Error { get; private set; }
        public static Response<T> Success(T data, int statusCode) {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful=true };
        }
        // default keywordu ile boş olucak ama status codu dolu olacak, ürün update edildiğinde ya da bir ürün silindiğinde 
        // veya herhangi bir entity de güncelleme ya da silme yaptığımızda bu endpoitnlerden geriye  data dönmeye gerek olmaz 
        // yani silinen  datayı yeniden dönmeye gerek yok. "200" durum kodu ile birlikte boş data dönelim.
        // request ile responsive arasında extra bir data alışverişine gerek yoktur.
        // ama ürün ekleme yani add işelminde geriye bir data dönmemiz gerekmektedir.


        // isSuccessfol : kendi iç yapımızda kullanılacaktır. Kısa yoldan olay başarılı mı değil mi diye kontrol etmek için direk olarak isSuccessful üzerinden kpntrol edilir.
        // not: Serialize edilmesini engellemeke için JsonIgnore keywordu kullanılmaktadır.

        [JsonIgnore]
        public bool IsSuccessful { get; private set; }
        public static Response<T> Success(int statusCode) {
            return new Response<T> { Data = default, StatusCode = statusCode, IsSuccessful=true };
        }

        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T>
            {
                Error = errorDto,
                StatusCode = statusCode,
                IsSuccessful=false
            };
        }

        // aşağıda dto nesnesi alınmadığından dolayı isShow verilmiştir. 
        public static Response<T> Fail(string errorMessage, int statusCode, bool isShow)
        {
            var errorDto =new  ErrorDto(errorMessage, isShow);
            return new Response<T> { Error = errorDto, StatusCode = statusCode,IsSuccessful=false };
        }
    }
}
