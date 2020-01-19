using System;

namespace InversionOfControl
{
    /// <summary>
    /// The chain of dependencies that a service depends on.
    /// </summary>
    public class DependencyChain
    {
        /// <summary>
        /// The parent node in the Dependency Chain.
        /// </summary>
        public DependencyChain Parent { get; }

        /// <summary>
        /// The type for the service used in this dependency chain.
        /// </summary>
        public Type Type { get; }

        public DependencyChain(Type type) => Type = type;

        public DependencyChain(Type type, DependencyChain parent) : this(type)
            => Parent = parent;
    }
}
