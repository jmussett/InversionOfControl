﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InversionOfControl
{
    /// <summary>
    /// The default implementation of the IServiceActivator interface
    /// </summary>
    public class DefaultContainerBackend : IContainerBackend
    {
        public IScopeContext CreateScopeContext() => new DefaultScopeContext();

        public TService CreateService<TService>(IEnumerable<object> services)
        {
            var type = typeof(TService);
            var service = CreateService(type, services);

            if (service == null)
                throw new MissingServiceException(type);

            return (TService) service;
        }

        private object CreateService(Type type, IEnumerable<object> services)
        {
            // Check if the type is a list and attempt 
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericArguments().First();

                // We need to construct the list type from the generic type argument of the service.
                var listType = typeof(List<>).MakeGenericType(genericType);

                // If a list can be assigned to the service type,
                // create a list from resolved services and return it.
                if (type.IsAssignableFrom(listType))
                {
                    var list = (IList) Activator.CreateInstance(listType);

                    // We then need to add all resolved services to that list.
                    foreach (var service in services)
                        list.Add(service);

                    return list;
                }
            }

            return services.FirstOrDefault();
        }

        public object ActivateInstance(ServiceRegistration registration, DependencyChain chain, IContainerVisitor visitor)
        {
            // Check dependency chain to see if we have a circular dependency.
            if (DetectCycle(chain))
                throw new CircularDependencyException(chain.Type, GetResolutionStack(chain));

            // If a factory method is defined, invoke it to activate the service.
            if (registration.FactoryMethod != null)
                return visitor.InvokeServiceFactory(registration.FactoryMethod);

            var concreteType = registration.ConcreteType;

            // If the concrete type is a generic type definition, we cannot invoke it's constructor.
            // We need to construct the type using the generic arguments defined on the requested type from the dependency chain.
            if (concreteType.IsGenericTypeDefinition)
                concreteType = concreteType.MakeGenericType(chain.Type.GetGenericArguments());

            // Order by constructors with the most amount of parameters.
            var constructors = concreteType.GetConstructors()
                .OrderByDescending(x => x.GetParameters().Length);

            Type missingService = null;

            foreach (var constructor in constructors)
            {
                // If we're interating through a new constructor, set the missing service to null.
                missingService = null;

                var paramInstances = new List<object>();

                // Iterate through all the parameters in the constructor.
                foreach (var paramInfo in constructor.GetParameters())
                {
                    // Create a new DependencyChain node for the parameter type.
                    var childChain = new DependencyChain(paramInfo.ParameterType, chain);

                    // Attempt to locate the services for the parameter.
                    var services = visitor.LocateServices(childChain);

                    // Create the parameter from the resolved services.
                    var parameter = CreateService(paramInfo.ParameterType, services);

                    // If the service for the parameter was not found, mark it as missing.
                    if (parameter == null)
                    {
                        missingService = paramInfo.ParameterType;
                        break;
                    }

                    paramInstances.Add(parameter);
                }

                // If a parameter was missing, move to the next constructor.
                if (missingService != null)
                    continue;

                // We have a valid constructor with instanciated parameters, time to invoke it.
                return constructor.Invoke(paramInstances.ToArray());
            }

            // Throw exception if a service was missing for the last constructor.
            if (missingService != null)
                throw new MissingDependencyException(missingService, registration.ConcreteType, GetResolutionStack(chain));

            // If we reach this code path, no valid public constructors were found.
            throw new MissingConstructorException(registration.ConcreteType);
        }

        // We produce the resolution stack by recursively interating through the dependency chain nodes.
        private string[] GetResolutionStack(DependencyChain chain)
        {
            var resolutionStack = new List<string> { chain.Type.FullName };

            // Attempt to get the resultion stack of the parent node and append it to the current stack.
            if (chain.Parent != null)
            {
                var parentStack = GetResolutionStack(chain.Parent);

                resolutionStack.AddRange(parentStack);
            }

            return resolutionStack.ToArray();
        }

        // We attempt to detect a circular dependency by recursively interating through the types in the chain.
        private bool DetectCycle(DependencyChain chain)
        {
            var types = new List<Type>();

            return CheckDependency(chain, types);
        }

        private bool CheckDependency(DependencyChain chain, List<Type> types)
        {
            // If the list of types contains the current type in the dependency, we have a circular reference.
            if (types.Contains(chain.Type))
                return true;

            types.Add(chain.Type);

            // If the chain node has a parent, check the parent type.
            if (chain.Parent != null)
                return CheckDependency(chain.Parent, types);

            // We've reached the top node, no circular reference found.
            return false;
        }
    }
}