using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InversionOfControl
{
    /// <summary>
    /// The default implementation of the IContainerBackend interface
    /// </summary>
    public class DefaultContainerBackend : IContainerBackend
    {
        private readonly IDictionary<Type, ConstructorExpression> _constructorCache
            = new Dictionary<Type, ConstructorExpression>();

        public virtual IScopeContext CreateScopeContext() => new DefaultScopeContext();

        public virtual object CreateService(Type type, IEnumerable<object> services)
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

        public virtual object ActivateInstance(ServiceRegistration registration, DependencyChain chain, IContainerVisitor visitor)
        {
            // Check dependency chain to see if we have a circular dependency.
            if (DetectCycle(chain))
                throw new CircularDependencyException(chain.Type, GetResolutionStack(chain));

            // If a factory method is defined, invoke it to activate the service.
            if (registration.FactoryMethod != null)
                return visitor.InvokeServiceFactory(registration.FactoryMethod);

            // Check the cache to see if the constructor expression was previously built.
            if (!_constructorCache.TryGetValue(registration.ServiceType, out var expression))
            {
                // Constructor expression hasn't been cached, we need to build it.
                expression = BuildConstructorExpression(registration.ConcreteType, chain);

                _constructorCache.Add(registration.ServiceType, expression);
            }

            var parameters = new object[expression.ParameterChains.Length];

            for(var i = 0; i < expression.ParameterChains.Length; i++)
            {
                var paramChain = expression.ParameterChains[i];

                // Attempt to locate the services for the parameter.
                var services = visitor.LocateServices(paramChain);

                // Create the parameter from the resolved services.
                var parameter = CreateService(paramChain.Type, services);

                parameters[i] = parameter
                    ?? throw new MissingDependencyException(paramChain.Type, chain.Type, GetResolutionStack(paramChain));
            }

            return expression.Activate(parameters);
        }

        private ConstructorExpression BuildConstructorExpression(Type concreteType, DependencyChain chain)
        {
            // If the concrete type is a generic type definition, we cannot invoke it's constructor.
            // We need to construct the type using the generic arguments defined on the requested type from the dependency chain.
            if (concreteType.IsGenericTypeDefinition)
                concreteType = concreteType.MakeGenericType(chain.Type.GetGenericArguments());

            // Order by constructors with the most amount of parameters.
            var constructors = concreteType.GetConstructors();

            var expression = new ConstructorExpression();

            // Don't bother ordering if only 1 constructor is present.
            if (constructors.Length == 1)
                return BuildConstructor(constructors[0], chain);

            var constructor = constructors
                .OrderByDescending(x => x.GetParameters().Length)
                .FirstOrDefault();

            // No valid public constructors were found.
            if (constructor == null)
                throw new MissingConstructorException(concreteType);

            return BuildConstructor(constructor, chain);
        }

        private ConstructorExpression BuildConstructor(ConstructorInfo constructor, DependencyChain chain)
        {
            var parameters = constructor.GetParameters();

            var expression = new ConstructorExpression
            {
                ParameterChains = new DependencyChain[parameters.Length]
            };

            // Iterate through all the parameters in the constructor.
            for (var i = 0; i < parameters.Length; i++)
            {
                // Create a new DependencyChain node for the parameter type.
                var childChain = new DependencyChain(parameters[i].ParameterType, chain);

                expression.ParameterChains[i] = childChain;
            }

            expression.BuildExpression(constructor);

            return expression;
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
            var types = new HashSet<Type>();

            return CheckDependency(chain, types);
        }

        private bool CheckDependency(DependencyChain chain, HashSet<Type> types)
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
