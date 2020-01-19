namespace InversionOfControl
{
    /// <summary>
    /// A dependency container used for retrieving and instanciating services.
    /// </summary>
    public interface IContainerServices
    {
        /// <summary>
        /// Retrieves and instanciates the instance of a service registered to the requested type.
        /// </summary>
        TService GetService<TService>();
    }
}
