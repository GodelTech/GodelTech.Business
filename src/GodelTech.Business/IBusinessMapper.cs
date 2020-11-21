namespace GodelTech.Business
{
    /// <summary>
    /// Interface of business mapper.
    /// </summary>
    public interface IBusinessMapper
    {
        /// <summary>
        /// Method to map from a source object using the provided mapping engine.
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Object source</param>
        /// <returns>Destination item</returns>
        TDestination Map<TDestination>(object source);

        /// <summary>
        /// Method to map from a source to destination using the provided mapping engine.
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source item</param>
        /// <param name="destination">Destination item</param>
        /// <returns>Destination item</returns>
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}
