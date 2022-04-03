using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    // nesne newlenmesi ietenir ise TDto:new yapmamız yeterli olacaktır. Sadece class olarak belirleyeceksek ise aşaoğıdaki gibi yazmamız yeterli olacaktır.
    public interface IServiceGeneric<TEntity,TDto> where TEntity:class where TDto:class
    {
        Task<Response<TDto>> GetByIdAsync(int id);
        Task<Response<IEnumerable<TDto>>> GelAllAsync();
        Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate);

        Task <Response<TDto>>AddAsync(TEntity entity);
        Task<Response<TDto>> Remove(TEntity entity);

        // bir nesne update edildiğinde zaten client tarafındaolduğu için tekrar bir update nesnesi dönmemize gerek yoktur.
        Task<Response<NoDataDto>> Update(TEntity entity);

   

    }
}
