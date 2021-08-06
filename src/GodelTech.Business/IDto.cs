namespace GodelTech.Business
{
    /// <summary>
    /// Interface of data transfer object.
    /// </summary>
    /// <typeparam name="TKey">The type of the T key.</typeparam>
    public interface IDto<TKey>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        TKey Id { get; set; }
    }
}