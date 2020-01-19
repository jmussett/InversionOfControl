namespace InversionOfControl
{
    /// <summary>
    /// A factory class used for instantiating new service contexts.
    /// Used when creating a new container scope.
    /// </summary>
    public interface IServiceContextFactory
    {
        /// <summary>
        /// Instanciates a new service context.
        /// </summary>
        IServiceContext CreateContext();
    }
}
