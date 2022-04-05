using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnıtOfWork;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class ServiceGeneric<TEntity, TDto> : IServiceGeneric<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IGenericRepository<TEntity> _genericRepository;

        public ServiceGeneric (IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity);

            await _genericRepository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);

            // aşağıda başarılı ise 200 durum kodu geçerli olacaktır.

            return Response<TDto>.Success(newDto, 200);

        }

        public async Task<Response<IEnumerable<TDto>>> GelAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<List<TDto>>(await  _genericRepository.GelAllAsync());
            return Response<IEnumerable<TDto>>.Success(products, 200);


        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product = await _genericRepository.GetByIdAsync(id);

            if (product==null)
            {
                return Response<TDto>.Fail("Id Not Found", 404, true);

            }
            return Response<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(product), 200);
        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
            var isExistsEntity = await _genericRepository.GetByIdAsync(id);

            if (isExistsEntity==null)
            {
                return Response<NoDataDto>.Fail("Id Not Found", 404, true);
            }
            _genericRepository.Remove(isExistsEntity); ;
            await _unitOfWork.CommitAsync();

            // 204 noContent status koduna karşılık gelmektedir. Body içerisinde herhangi bir data olmayacaktır.
            return Response<NoDataDto>.Success(204);


        }


        
        public async Task<Response<NoDataDto>> Update(TDto entity, int id)
        {
            var isExistsEntity = await _genericRepository.GetByIdAsync(id);

            if (isExistsEntity == null)
            {
                return Response<NoDataDto>.Fail("Id Not Found", 404, true);
            }

            var updateEntity = ObjectMapper.Mapper.Map<TEntity>(entity);

            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            // where(x=>x.id>5)
            var list = _genericRepository.Where(predicate);
            return Response<IEnumerable<TDto>>.Success(ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await list.ToListAsync()),200);
        }
    }
}
