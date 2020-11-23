using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodelTech.Business
{
    /// <summary>
    /// Interface of service for business layer.
    /// </summary>
    /// <typeparam name="TDto">The type of the T data transfer object.</typeparam>
    /// <typeparam name="TType">The type of the T type.</typeparam>
    public interface IService<TDto, TType>
        where TDto : class
    {
        /// <summary>
        /// Asynchronously gets data transfer object models of type T.
        /// </summary>
        /// <returns><cref>Task{IList{TDto}}</cref>.</returns>
        Task<IList<TDto>> GetListAsync();

        /// <summary>
        /// Asynchronously gets data transfer object model of type T.
        /// If no model is found, then null is returned.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><cref>Task{TDto}</cref>.</returns>
        Task<TDto> GetAsync(TType id);

        /// <summary>
        /// Asynchronously adds data transfer object.
        /// </summary>
        /// <typeparam name="TAddDto">The type of the T add data transfer object.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns><cref>TDto</cref>.</returns>
        Task<TDto> AddAsync<TAddDto>(TAddDto item)
            where TAddDto : class;

        /// <summary>
        /// Asynchronously updates data transfer object.
        /// </summary>
        /// <typeparam name="TEditDto">The type of the T edit data transfer object.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="item">The item.</param>
        /// <returns>TDto.</returns>
        Task<TDto> EditAsync<TEditDto>(TType id, TEditDto item)
            where TEditDto : class;

        /// <summary>
        /// Asynchronously deletes the specified data transfer object.
        /// </summary>
        /// <param name="id">The identifier.</param>
        Task DeleteAsync(TType id);
    }
}
