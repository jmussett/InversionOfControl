namespace InversionOfControl
{
    /// <summary>
    /// An interface used for retrieving services and creating scopes at runtime.
    /// </summary>
    public interface IContainerRuntime : IContainerServices
    {
        /// <summary>
        /// Creates a new scope that has it's own lifetime.
        /// </summary>
        IContainerScope CreateScope();
    }
}
