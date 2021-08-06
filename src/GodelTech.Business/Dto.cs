namespace GodelTech.Business
{
    /// <summary>
    /// Data transfer object.
    /// </summary>
    /// <typeparam name="TKey">The type of the T key.</typeparam>
    public class Dto<TKey> : IDto<TKey>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public virtual TKey Id { get; set; }
    }
}