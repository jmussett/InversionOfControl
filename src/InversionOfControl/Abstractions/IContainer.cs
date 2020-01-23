using System;

namespace InversionOfControl
{
    /// <summary>
    /// A dependency container used for retrieving and instanciating services.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Retrieves and instanciates the instance of a service registered to the requested type.
        /// </summary>
        object GetService(Type type);
    }
}
