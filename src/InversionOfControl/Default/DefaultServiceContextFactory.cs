namespace InversionOfControl
{
    /// <summary>
    /// The default implementation of the IServiceContextFactory interface
    /// </summary>
    public class DefaultServiceContextFactory : IServiceContextFactory
    {
        public IServiceContext CreateContext() => new DefaultServiceContext();
    }
}
