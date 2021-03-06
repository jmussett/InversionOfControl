﻿using System;
using System.Collections.Generic;

namespace InversionOfControl
{
    /// <summary>
    /// Interface for backend implementation for container logic
    /// </summary>
    public interface IContainerBackend
    {
        /// <summary>
        /// Instanciates a new scope context.
        /// </summary>
        IScopeContext CreateScopeContext();

        /// <summary>
        /// Creates the single compiled service for all instances found for that service type.
        /// </summary>
        object CreateService(Type type, IEnumerable<object> services);

        /// <summary>
        /// Activates a new instance of a registered instance in a given location in the dependency chain.
        /// </summary>
        object ActivateInstance(ServiceRegistration registration, DependencyChain chain, IContainerVisitor visitor);

    }
}
