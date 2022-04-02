using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    // NOT: kapsamlı projelerde generic repository kodun daha karmaşık hale gelmesini sağlamaktadır
    // business kuralları fazla ise generic repository sıkıntılı olabilir.
    public interface IGenericRepository<TEntity> where TEntity:class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GelAllAsync();
        Task AddAsync(TEntity entity);
        // biz dbcontext de  remove metodu çağırdığımızda bunun aslında async metodu yoktur
        void Remove(TEntity entity);
        TEntity Update(TEntity entity);
        // contect.Entry(entity).state = EntityState.Modified
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

        //product = porductRepository.Where(x=>x.id>);
        //product.any
        //product.ToList

        // en son veritabanına ne zaman yansıtırsak o zaman ToList çalışır

    }
}
