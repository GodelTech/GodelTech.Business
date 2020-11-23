using GodelTech.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodelTech.Business
{
#pragma warning disable S2436
    /// <summary>
    /// Service for business layer.
    /// </summary>
    /// <typeparam name="TUnitOfWork">The type of the T unit of work.</typeparam>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    /// <typeparam name="TDto">The type of the T data transfer object.</typeparam>
    /// <typeparam name="TType">The type of the T type.</typeparam>
    public class Service<TUnitOfWork, TEntity, TDto, TType> : IService<TDto, TType>
        where TUnitOfWork : IUnitOfWork
        where TEntity : class, IEntity<TType>
        where TDto : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Service{TUnitOfWork, IBusinessMapper}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="businessMapper">The business mapper.</param>
        public Service(TUnitOfWork unitOfWork, IBusinessMapper businessMapper)
        {
            UnitOfWork = unitOfWork;
            BusinessMapper = businessMapper;
        }

        /// <summary>
        /// Gets the unit of work.
        /// </summary>
        /// <value>The unit of work.</value>
        protected TUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Gets the business mapper.
        /// </summary>
        /// <value>The business mapper.</value>
        protected IBusinessMapper BusinessMapper { get; }

        /// <summary>
        /// Asynchronously gets data transfer object models of type T.
        /// </summary>
        /// <returns><cref>Task{IList{TDto}}</cref>.</returns>
        public virtual async Task<IList<TDto>> GetListAsync()
        {
            return await UnitOfWork
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
            return await UnitOfWork
                .GetRepository<TEntity, TType>()
                .GetAsync<TDto, TEntity, TType>(id);
        }

        /// <summary>
        /// Asynchronously adds data transfer object.
        /// </summary>
        /// <typeparam name="TAddDto">The type of the T add data transfer object.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns><cref>TDto</cref>.</returns>
        public virtual async Task<TDto> AddAsync<TAddDto>(TAddDto item)
            where TAddDto : class
        {
            var entity = BusinessMapper.Map<TEntity>(item);

            entity = await UnitOfWork
                .GetRepository<TEntity, TType>()
                .InsertAsync(entity);

            await UnitOfWork.CommitAsync();

            return BusinessMapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Asynchronously updates data transfer object.
        /// </summary>
        /// <typeparam name="TEditDto">The type of the T edit data transfer object.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="item">The item.</param>
        /// <returns>TDto.</returns>
        public virtual async Task<TDto> EditAsync<TEditDto>(TType id, TEditDto item)
            where TEditDto : class
        {
            var entity = await UnitOfWork
                .GetRepository<TEntity, TType>()
                .GetAsync(id);

            if (entity != null)
            {
                BusinessMapper.Map(item, entity);

                entity = UnitOfWork
                    .GetRepository<TEntity, TType>()
                    .Update(entity);

                await UnitOfWork.CommitAsync();
            }

            return BusinessMapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Asynchronously deletes the specified data transfer object.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public virtual async Task DeleteAsync(TType id)
        {
            UnitOfWork
                .GetRepository<TEntity, TType>()
                .Delete(id);

            await UnitOfWork.CommitAsync();
        }
    }
#pragma warning restore S2436
}
