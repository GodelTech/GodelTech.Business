using GodelTech.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodelTech.Business
{
    /// <summary>
    /// Service for business layer.
    /// </summary>
    /// <typeparam name="TUnitOfWork">The type of the T unit of work.</typeparam>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    /// <typeparam name="TDto">The type of the T data transfer object.</typeparam>
    /// <typeparam name="TAddDto">The type of the T add data transfer object.</typeparam>
    /// <typeparam name="TType">The type of the T type.</typeparam>
    public class Service<TUnitOfWork, TEntity, TDto, TAddDto, TType> : IService<TDto, TAddDto, TType>
        where TUnitOfWork : IUnitOfWork
        where TEntity : class, IEntity<TType>
        where TDto : class
        where TAddDto : class
    {
        private readonly TUnitOfWork _unitOfWork;
        private readonly IBusinessMapper _businessMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="businessMapper">The business mapper.</param>
        public Service(TUnitOfWork unitOfWork, IBusinessMapper businessMapper)
        {
            _unitOfWork = unitOfWork;
            _businessMapper = businessMapper;
        }

        /// <summary>
        /// Asynchronously gets data transfer object models of type T.
        /// </summary>
        /// <returns><cref>Task{IList{TDto}}</cref>.</returns>
        public virtual async Task<IList<TDto>> GetListAsync()
        {
            return await _unitOfWork
                .GetRepository<TEntity, TType>()
                .GetListAsync<TDto, TEntity, TType>(null);
        }

        /// <summary>
        /// Asynchronously gets data transfer object model of type T.
        /// If no model is found, then null is returned.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><cref>Task{TDto}</cref>.</returns>
        public virtual async Task<TDto> GetAsync(TType id)
        {
            return await _unitOfWork
                .GetRepository<TEntity, TType>()
                .GetAsync<TDto, TEntity, TType>(id);
        }

        /// <summary>
        /// Asynchronously adds data transfer object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><cref>TDto</cref>.</returns>
        public virtual async Task<TDto> AddAsync(TAddDto item)
        {
            var entity = _businessMapper.Map<TEntity>(item);

            entity = await _unitOfWork
                .GetRepository<TEntity, TType>()
                .InsertAsync(entity);

            await _unitOfWork.CommitAsync();

            return _businessMapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Asynchronously updates data transfer object.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="item">The item.</param>
        /// <returns>TDto.</returns>
        public virtual async Task<TDto> EditAsync(TType id, TDto item)
        {
            var entity = await _unitOfWork
                .GetRepository<TEntity, TType>()
                .GetAsync(id);

            if (entity != null)
            {
                _businessMapper.Map(item, entity);

                entity = _unitOfWork
                    .GetRepository<TEntity, TType>()
                    .Update(entity);

                await _unitOfWork.CommitAsync();
            }

            return _businessMapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Asynchronously deletes the specified data transfer object.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public virtual async Task DeleteAsync(TType id)
        {
            _unitOfWork
                .GetRepository<TEntity, TType>()
                .Delete(id);

            await _unitOfWork.CommitAsync();
        }
    }
}
