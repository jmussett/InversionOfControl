using System;

namespace InversionOfControl
{
    /// <summary>
    /// A container scope containing services that can be disposed of that have their own lifetime.
    /// </summary>
    public interface IContainerScope : IDisposable, IContainer { }
}
