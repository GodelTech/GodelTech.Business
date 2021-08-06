using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodelTech.Business
{
    /// <summary>
    /// Interface of service for business layer.
    /// </summary>
    /// <typeparam name="TDto">The type of the T data transfer object.</typeparam>
    /// <typeparam name="TAddDto">The type of the T type.</typeparam>
    /// <typeparam name="TEditDto">The type of the T type.</typeparam>
    /// <typeparam name="TType">The type of the T type.</typeparam>
#pragma warning disable S2436 // Reduce the number of generic parameters in the 'IBusinessService' interface to no more than the 2 authorized.
    public interface IBusinessService<TDto, in TAddDto, in TEditDto, in TType>
#pragma warning restore S2436 // Reduce the number of generic parameters in the 'IBusinessService' interface to no more than the 2 authorized.
        where TDto : class, IDto<TType>
        where TAddDto : class
        where TEditDto : class, IDto<TType>
    {
        /// <summary>
        /// Asynchronously gets data transfer object models of type T data transfer object.
        /// </summary>
        /// <returns><cref>Task{IList{TDto}}</cref>.</returns>
        Task<IList<TDto>> GetListAsync();

        /// <summary>
        /// Asynchronously gets data transfer object model of type T data transfer object.
        /// If no model is found, then null is returned.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><cref>Task{TDto}</cref>.</returns>
        Task<TDto> GetAsync(TType id);

        /// <summary>
        /// Asynchronously adds data transfer object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><cref>TDto</cref>.</returns>
        Task<TDto> AddAsync(TAddDto item);

        /// <summary>
        /// Asynchronously updates data transfer object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>TDto.</returns>
        Task<TDto> EditAsync(TEditDto item);

        /// <summary>
        /// Asynchronously deletes the specified data transfer object.
        /// </summary>
        /// <param name="id">The identifier.</param>
        Task<bool> DeleteAsync(TType id);
    }
}