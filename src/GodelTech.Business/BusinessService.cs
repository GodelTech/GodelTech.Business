using System;
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
    /// <typeparam name="TKey">The type of the T key.</typeparam>
    /// <typeparam name="TUnitOfWork">The type of the T UnitOfWork.</typeparam>
    /// <typeparam name="TDto">The type of the T data transfer object.</typeparam>
    /// <typeparam name="TAddDto">The type of the T add data transfer object.</typeparam>
    /// <typeparam name="TEditDto">The type of the T edit data transfer object.</typeparam>
    public abstract class BusinessService<TEntity, TKey, TUnitOfWork, TDto, TAddDto, TEditDto>
        : IBusinessService<TDto, TAddDto, TEditDto, TKey>
        where TEntity : class, IEntity<TKey>
        where TUnitOfWork : class, IUnitOfWork
        where TDto : class, IDto<TKey>
        where TAddDto : class
        where TEditDto : class, IDto<TKey>
    {
        private readonly Func<TUnitOfWork, IRepository<TEntity, TKey>> _repositorySelector;
        private readonly IBusinessMapper _businessMapper;

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
        protected IRepository<TEntity, TKey> Repository => _repositorySelector(UnitOfWork);

        /// <summary>
        /// Logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Asynchronously gets data transfer object models of type T data transfer object.
        /// </summary>
        /// <returns><cref>Task{IList{TDto}}</cref>.</returns>
        public async Task<IList<TDto>> GetListAsync()
        {
            return await Repository
                .GetListAsync<TDto, TEntity, TKey>();
        }

        /// <summary>
        /// Asynchronously gets data transfer object model of type T data transfer object.
        /// If no model is found, then null is returned.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><cref>Task{TDto}</cref>.</returns>
        public async Task<TDto> GetAsync(TKey id)
        {
            return await Repository
                .GetAsync<TDto, TEntity, TKey>(id);
        }

        /// <summary>
        /// Asynchronously adds data transfer object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><cref>TDto</cref>.</returns>
        public Task<TDto> AddAsync(TAddDto item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return AddInternalAsync(item);
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
        public async Task<bool> DeleteAsync(TKey id)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                LogDeleteAsyncDeleteItemInformationCallback(Logger, id, null);
            }

            Repository.Delete(id);

            if (Logger.IsEnabled(LogLevel.Information))
            {
                _logDeleteAsyncSaveChangesInformationCallback(Logger, null);
            }
            var result = await UnitOfWork.CommitAsync();

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

        private async Task<TDto> AddInternalAsync(TAddDto item)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                LogAddInternalAsyncAddItemInformationCallback(Logger, item, null);
            }

            var entity = _businessMapper.Map<TAddDto, TEntity>(item);

            entity = await Repository
                .InsertAsync(entity);

            if (Logger.IsEnabled(LogLevel.Information))
            {
                LogAddInternalAsyncSaveChangesInformationCallback(Logger, null);
            }

            await UnitOfWork.CommitAsync();

            return _businessMapper.Map<TEntity, TDto>(entity);
        }

        private static readonly Action<ILogger, TEditDto, Exception> LogEditInternalAsyncEditItemInformationCallback
            = LoggerMessage.Define<TEditDto>(
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

        private async Task<TDto> EditInternalAsync(TEditDto item)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                LogEditInternalAsyncEditItemInformationCallback(Logger, item, null);
            }

            var entity = await Repository
                .GetAsync(item.Id);

            if (entity == null)
            {
                if (Logger.IsEnabled(LogLevel.Warning))
                {
                    LogEditInternalAsyncItemNotFoundWarningCallback(
                        Logger,
                        item.Id,
                        null
                    );
                }

                return null;
            }

            _businessMapper.Map(item, entity);

            entity = Repository.Update(entity);

            if (Logger.IsEnabled(LogLevel.Information))
            {
                LogEditInternalAsyncSaveChangesInformationCallback(Logger, null);
            }

            await UnitOfWork.CommitAsync();

            return _businessMapper.Map<TEntity, TDto>(entity);
        }
    }
}