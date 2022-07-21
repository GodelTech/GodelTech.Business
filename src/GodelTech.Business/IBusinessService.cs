using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GodelTech.Business
{
    /// <summary>
    /// Interface of service for business layer.
    /// </summary>
    /// <typeparam name="TDto">The type of the T data transfer object.</typeparam>
    /// <typeparam name="TAddDto">The type of the T type.</typeparam>
    /// <typeparam name="TEditDto">The type of the T type.</typeparam>
    /// <typeparam name="TKey">The type of the T key.</typeparam>
#pragma warning disable S2436 // Reduce the number of generic parameters in the 'IBusinessService' interface to no more than the 2 authorized.
    public interface IBusinessService<TDto, in TAddDto, in TEditDto, in TKey>
#pragma warning restore S2436 // Reduce the number of generic parameters in the 'IBusinessService' interface to no more than the 2 authorized.
        where TDto : class, IDto<TKey>
        where TAddDto : class
        where TEditDto : class, IDto<TKey>
    {
        /// <summary>
        /// Asynchronously gets data transfer object models of type T data transfer object.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns><cref>Task{IList{TDto}}</cref>.</returns>
        Task<IList<TDto>> GetListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously gets data transfer object model of type T data transfer object.
        /// If no model is found, then null is returned.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns><cref>Task{TDto}</cref>.</returns>
        Task<TDto> GetAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously adds data transfer object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns><cref>TDto</cref>.</returns>
        Task<TDto> AddAsync(TAddDto item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously updates data transfer object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>TDto.</returns>
        Task<TDto> EditAsync(TEditDto item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously deletes the specified data transfer object.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Boolean result.</returns>
        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
    }
}
