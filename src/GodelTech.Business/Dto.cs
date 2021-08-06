namespace GodelTech.Business
{
    /// <summary>
    /// Data transfer object.
    /// </summary>
    /// <typeparam name="TType">The type of the T type.</typeparam>
    public class Dto<TType> : IDto<TType>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public virtual TType Id { get; set; }
    }
}