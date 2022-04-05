using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<Tentity> : IGenericRepository<Tentity> where Tentity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<Tentity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Tentity>();
        }

        // aşağıdaki işlem ile birlikte memory de  1 tane entity eklenmiştir. Veritabanına yansımamıştır. 
        // UnıtOfWork ile birlikte SaveChanges metodu çalıştırdığımızda o zaman veri tabanına yansıtmaktadır.

        // NOT: ef otomatik roleback dönmektedir.

        public async Task AddAsync(Tentity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<Tentity>> GelAllAsync()
        {
            return await _dbSet.ToListAsync();
        }


        // FindAsync() parantez içindeki "params" ben buraya birden fazla değer döndürebilirim anlamını taşımaktadır.
        public async Task<Tentity> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity!=null)
            {
                // Detached ile birlikte memoride takip edilmez. Memory de therad edilmez
                // Entity.Detached yapısı service class'ında daha detaylı bir şekilde belli olacaktır.
                // null olan bir şeyi detached yaparsak hata verir
                _context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }
        

        public void Remove(Tentity entity)
        {
            _dbSet.Remove(entity);
        }

        public Tentity Update(Tentity entity)
        {
            // Repository pattern in kullanılmamasının dezavantajlarından birisi, bir tane property dahi değiştirsek ve biz modified olarak...
            // işaretlediğimizde tüm alanları günceller. 


            //Product.getbyId(1)
            //product.name="kalem"
            //context.sace
            _context.Entry(entity).State = EntityState.Modified;

            return entity;
        }

        // 
        public IQueryable<Tentity> Where(Expression<Func<Tentity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
}
