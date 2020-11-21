namespace GodelTech.Business
{
    /// <summary>
    /// Interface of data transfer object for business layer.
    /// </summary>
    /// <typeparam name="TType">The type of the T type.</typeparam>
    public interface IDto<TType>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        TType Id { get; set; }
    }
}
