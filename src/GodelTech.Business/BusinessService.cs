﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.Data;
using GodelTech.Data.Extensions;
using Microsoft.Extensions.Logging;

[assembly: CLSCompliant(false)]
namespace GodelTech.Business
{
    /// <summary>
    /// Business service.
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    /// <typeparam name="TType">The type of the T type.</typeparam>
    /// <typeparam name="TUnitOfWork">The type of the T UnitOfWork.</typeparam>
    /// <typeparam name="TDto">The type of the T data transfer object.</typeparam>
    /// <typeparam name="TAddDto">The type of the T add data transfer object.</typeparam>
    /// <typeparam name="TEditDto">The type of the T edit data transfer object.</typeparam>
#pragma warning disable S2436 // Reduce the number of generic parameters in the 'BusinessService' class to no more than the 2 authorized.
    public abstract class BusinessService<TEntity, TType, TUnitOfWork, TDto, TAddDto, TEditDto>
#pragma warning restore S2436 // Reduce the number of generic parameters in the 'BusinessService' class to no more than the 2 authorized.
        : IBusinessService<TDto, TAddDto, TEditDto, TType>
        where TEntity : class, IEntity<TType>
        where TUnitOfWork : class, IUnitOfWork
        where TDto : class, IDto<TType>
        where TAddDto : class
        where TEditDto : class, IDto<TType>
    {
        private readonly Func<TUnitOfWork, IRepository<TEntity, TType>> _repositorySelector;
        private readonly IBusinessMapper _businessMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessService{TEntity, TType, TUnitOfWork, TDto, TAddDto, TEditDto}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The UnitOfWork.</param>
        /// <param name="repositorySelector">The repository selector function.</param>
        /// <param name="businessMapper">The business mapper.</param>
        /// <param name="logger">The logger.</param>
        protected BusinessService(
            TUnitOfWork unitOfWork,
            Func<TUnitOfWork, IRepository<TEntity, TType>> repositorySelector,
            IBusinessMapper businessMapper,
            ILogger<BusinessService<TEntity, TType, TUnitOfWork, TDto, TAddDto, TEditDto>> logger)
        {
            UnitOfWork = unitOfWork;
            _repositorySelector = repositorySelector;
            _businessMapper = businessMapper;
            Logger = logger;
        }

        /// <summary>
        /// UnitOfWork.
        /// </summary>
        protected TUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Repository.
        /// </summary>
        protected IRepository<TEntity, TType> Repository => _repositorySelector(UnitOfWork);

        /// <summary>
        /// Logger.
        /// </summary>
        protected ILogger<BusinessService<TEntity, TType, TUnitOfWork, TDto, TAddDto, TEditDto>> Logger { get; }

        /// <summary>
        /// Asynchronously gets data transfer object models of type T data transfer object.
        /// </summary>
        /// <returns><cref>Task{IList{TDto}}</cref>.</returns>
        public async Task<IList<TDto>> GetListAsync()
        {
            return await Repository
                .GetListAsync<TDto, TEntity, TType>();
        }

        /// <summary>
        /// Asynchronously gets data transfer object model of type T data transfer object.
        /// If no model is found, then null is returned.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><cref>Task{TDto}</cref>.</returns>
        public async Task<TDto> GetAsync(TType id)
        {
            return await Repository
                .GetAsync<TDto, TEntity, TType>(id);
        }

        /// <summary>
        /// Asynchronously adds data transfer object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><cref>TDto</cref>.</returns>
        public async Task<TDto> AddAsync(TAddDto item)
        {
            Logger.LogInformation($"Add item: {item}");

            var entity = _businessMapper.Map<TAddDto, TEntity>(item);

            entity = await Repository
                .InsertAsync(entity);

            Logger.LogInformation("Save changes");
            await UnitOfWork.CommitAsync();

            return _businessMapper.Map<TEntity, TDto>(entity);
        }

        /// <summary>
        /// Asynchronously updates data transfer object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>TDto.</returns>
        public Task<TDto> EditAsync(TEditDto item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return EditInternalAsync(item);
        }

        /// <summary>
        /// Asynchronously deletes the specified data transfer object.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public async Task<bool> DeleteAsync(TType id)
        {
            Logger.LogInformation($"Delete item: {id}");

            Repository.Delete(id);

            Logger.LogInformation("Save changes");
            var result = await UnitOfWork.CommitAsync();

            return result == 1;
        }

        private async Task<TDto> EditInternalAsync(TEditDto item)
        {
            Logger.LogInformation($"Edit item: {item}");

            var entity = await Repository
                .GetAsync(item.Id);

            if (entity == null)
            {
                Logger.LogWarning($"Item not found: {item.Id}");
                return null;
            }

            _businessMapper.Map(item, entity);

            entity = Repository
                .Update(entity);

            Logger.LogInformation("Save changes");
            await UnitOfWork.CommitAsync();

            return _businessMapper.Map<TEntity, TDto>(entity);
        }
    }
}