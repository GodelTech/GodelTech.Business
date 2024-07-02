using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.Data;
using Microsoft.Extensions.Logging;

[assembly: CLSCompliant(false)]
namespace GodelTech.Business
{
    /// <summary>
    /// Business service.
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    /// <typeparam name="TKey">The type of the T key.</typeparam>
    /// <typeparam name="TUnitOfWork">The type of the T UnitOfWork.</typeparam>
    /// <typeparam name="TDto">The type of the T data transfer object.</typeparam>
    /// <typeparam name="TAddDto">The type of the T add data transfer object.</typeparam>
    /// <typeparam name="TEditDto">The type of the T edit data transfer object.</typeparam>
#pragma warning disable S2436 // Reduce the number of generic parameters in the 'BusinessService' class to no more than the 2 authorized.
    public abstract class BusinessService<TEntity, TKey, TUnitOfWork, TDto, TAddDto, TEditDto>
#pragma warning restore S2436 // Reduce the number of generic parameters in the 'BusinessService' class to no more than the 2 authorized.
        : IBusinessService<TDto, TAddDto, TEditDto, TKey>
        where TEntity : class, IEntity<TKey>
        where TUnitOfWork : class, IUnitOfWork
        where TDto : class, IDto<TKey>
        where TAddDto : class
        where TEditDto : class, IDto<TKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessService{TEntity, TKey, TUnitOfWork, TDto, TAddDto, TEditDto}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The UnitOfWork.</param>
        /// <param name="repositorySelector">The repository selector function.</param>
        /// <param name="businessMapper">The business mapper.</param>
        /// <param name="logger">The logger.</param>
        protected BusinessService(
            TUnitOfWork unitOfWork,
            Func<TUnitOfWork, IRepository<TEntity, TKey>> repositorySelector,
            IBusinessMapper businessMapper,
            ILogger logger)
        {
            if (repositorySelector == null) throw new ArgumentNullException(nameof(repositorySelector));

            UnitOfWork = unitOfWork;
            Repository = repositorySelector(unitOfWork);
            BusinessMapper = businessMapper;
            Logger = logger;
        }

        /// <summary>
        /// UnitOfWork.
        /// </summary>
        protected TUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Repository.
        /// </summary>
        protected IRepository<TEntity, TKey> Repository { get; }

        /// <summary>
        /// Gets the business mapper.
        /// </summary>
        /// <value>The business mapper.</value>
        protected IBusinessMapper BusinessMapper { get; }

        /// <summary>
        /// Logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Asynchronously gets data transfer object models of type T data transfer object.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns><cref>Task{IList{TDto}}</cref>.</returns>
        public async Task<IList<TDto>> GetListAsync(CancellationToken cancellationToken = default)
        {
            return await Repository
                .GetListAsync<TDto, TEntity, TKey>(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Asynchronously gets data transfer object model of type T data transfer object.
        /// If no model is found, then null is returned.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns><cref>Task{TDto}</cref>.</returns>
        public async Task<TDto> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await Repository
                .GetAsync<TDto, TEntity, TKey>(id, cancellationToken);
        }

        /// <summary>
        /// Asynchronously adds data transfer object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns><cref>TDto</cref>.</returns>
        public Task<TDto> AddAsync(TAddDto item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return AddInternalAsync(item, cancellationToken);
        }

        /// <summary>
        /// Asynchronously updates data transfer object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>TDto.</returns>
        public Task<TDto> EditAsync(TEditDto item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return EditInternalAsync(item, cancellationToken);
        }

        private static readonly Action<ILogger, TKey, Exception> LogDeleteAsyncDeleteItemInformationCallback =
            LoggerMessage.Define<TKey>(
                LogLevel.Information,
                new EventId(0, nameof(DeleteAsync)),
                "Delete item: {Id}"
            );

        private readonly Action<ILogger, Exception> _logDeleteAsyncSaveChangesInformationCallback =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(0, nameof(DeleteAsync)),
                "Save changes"
            );

        /// <summary>
        /// Asynchronously deletes the specified data transfer object.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Boolean result.</returns>
        public async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            LogDeleteAsyncDeleteItemInformationCallback(Logger, id, null);

            await Repository.DeleteAsync(id, cancellationToken);

            _logDeleteAsyncSaveChangesInformationCallback(Logger, null);

            var result = await UnitOfWork.CommitAsync(cancellationToken);

            return result == 1;
        }

        private static readonly Action<ILogger, TAddDto, Exception> LogAddInternalAsyncAddItemInformationCallback =
            LoggerMessage.Define<TAddDto>(
                LogLevel.Information,
                new EventId(0, nameof(AddInternalAsync)),
                "Add item: {Item}"
            );

        private Action<ILogger, Exception> LogAddInternalAsyncSaveChangesInformationCallback { get; } =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(0, nameof(AddInternalAsync)),
                "Save changes"
            );

        private async Task<TDto> AddInternalAsync(TAddDto item, CancellationToken cancellationToken)
        {
            LogAddInternalAsyncAddItemInformationCallback(Logger, item, null);

            var entity = BusinessMapper.Map<TAddDto, TEntity>(item);

            entity = await Repository
                .InsertAsync(entity, cancellationToken);

            LogAddInternalAsyncSaveChangesInformationCallback(Logger, null);

            await UnitOfWork.CommitAsync(cancellationToken);

            return BusinessMapper.Map<TEntity, TDto>(entity);
        }

        private static readonly Action<ILogger, TEditDto, Exception> LogEditInternalAsyncEditItemInformationCallback =
            LoggerMessage.Define<TEditDto>(
                LogLevel.Information,
                new EventId(0, nameof(EditInternalAsync)),
                "Edit item: {Item}"
            );

        private static readonly Action<ILogger, TKey, Exception> LogEditInternalAsyncItemNotFoundWarningCallback =
            LoggerMessage.Define<TKey>(
                LogLevel.Warning,
                new EventId(0, nameof(EditInternalAsync)),
                "Item not found: {Item}"
            );

        private Action<ILogger, Exception> LogEditInternalAsyncSaveChangesInformationCallback { get; } =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(0, nameof(EditInternalAsync)),
                "Save changes"
            );

        private async Task<TDto> EditInternalAsync(TEditDto item, CancellationToken cancellationToken)
        {
            LogEditInternalAsyncEditItemInformationCallback(Logger, item, null);

            var entity = await Repository
                .GetAsync(item.Id, cancellationToken);

            if (entity == null)
            {
                LogEditInternalAsyncItemNotFoundWarningCallback(Logger, item.Id, null);
                return null;
            }

            BusinessMapper.Map(item, entity);

            entity = await Repository.UpdateAsync(entity, cancellationToken: cancellationToken);

            LogEditInternalAsyncSaveChangesInformationCallback(Logger, null);

            await UnitOfWork.CommitAsync(cancellationToken);

            return BusinessMapper.Map<TEntity, TDto>(entity);
        }
    }
}
