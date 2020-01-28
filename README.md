# Inversion Of Control
This is my attempt at implementing a fully functioning Dependency Injection framework from scratch.
It's inspired by [Microsoft's ASP .Net Core Dependancy Injection Framework](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)

## The Basics
This library manages the dependancies of classes (or services) automatically by allowing the user to register them to a container at the start of the application.
Services can be registered to the container and the runtime can be built like so:

```c#
var containerRuntime = new ContainerBuilder()
    .AddTransient<SomeClass>()
    .BuildRuntime();
```

Resolving a service is strait forward:
```c#
var service = containerRuntime.GetService<SomeClass>();
```

Sometimes, a seperate scope will need creating to limit the lifespan of scoped services registered:
```c#
var containerRuntime = new ContainerBuilder()
    .AddTransient<ScopedClass>()
    .BuildRuntime();

using(var scope = containerRuntime.CreateScope())
{
    var scopedService = scope.GetService<ScopedClass>();
}
```
If the scoped service registered implements IDisposable, the service will automatically be disposed of when it exists the scope.

## Registering Services

When registering a service, it's lifespan can be specified to determine when it can instantiate new instances. This is defined it the method name.
This dependancy injection framework supports 3 different lifespans:
* Transient - A new instance is created every time a service is requested. 
```c#
    .AddTransient<SomeClass>()
```
* Singleton - Only a single instance of the type is created.
```c#
    .AddSingleton<SomeClass>()
```
* Scoped - Only a single instance of the type is created per scope.
```c#
    .AddScoped<SomeClass>()
```

Interfaces with coresponding concrete types can be specified by adding an additional generic type parameter:
```c#
    .AddTransient<ISomeInterface, SomeClass>()
```

An instance of the class can be specified when registering it as a singleton:
```c#
    .AddSingleton<ISomeInterface>(new SomeClass())
```

Factory methods can be specified to instantiate the instance with a callback:
```c#
    .AddSingleton<ISomeInterface>(runtime => new SomeClass(runtime.GetService<SomeDependency>()))
```
The container runtime passed in above allows the resolution of other dependancies to resolved any required services.  

Unbound generic types can be registered, this is beneficial when registering generic type definitions without specified generic type parameters:
```c#
    .AddTransient(typeof(ISomeGenericInterface<>), typeof(SomeGenericClass<>))
```

Each service type can have multiple service types registered to it. If a single instance is requested, it will retrieve the implmentation for the first registration it can find.

For resolving multiple services for a single type, a dedicated method on the container can be used. Any interface that a List can be assigned to can be specified:
```c#
    container.GetService<IEnumerable<ISomeType>>();
    container.GetServices<ISomeType>();
```

## Constructors and Dependancies

As with any Dependancy Injection framework, an important aspect revolves around resolving the child dependancies for each given service.
The most straitforward way to pass through any dependancy to an object in C# is through the constructor. See this example below:
```c#
public class ServiceA
{
    public ServiceA(ServiceB serviceB) { }
}

public class ServiceB
{
    public ServiceB(ServiceC serviceB) { }
}

public class ServiceC { }
```
ServiceA depends on ServiceB being passed through to it's constructor, and ServiceB depends on ServiceC.
This frameworks resolves those dependancies automatically without having to instantiate them manually each time.
As long as they are all registered it will work as expected.

In C#, classes support multiple constructors. This framework also supports this.
Currently, the framework looks for the constructor with the most amount of parameters. If it fails to invoke that constructor due to missing dependancies, it will attempt to invoke the next constructor, and so on...

## Exception Handling

An important aspect to this framework is the way it manages exceptions.
There are 4 different exceptions that it currently throws:
* MissingServiceException - thrown when the required service that was requested was not registered in the container.
* MissingDependencyException - thrown when a dependancy for a service was not registered in the container.
* MissingConstructorException - thrown when no public constructors are available for a requested service.
* CircularDependencyException - thrown when a circular dependancy was found in the dependancy chain.

With MissingDependencyException and CircularDependencyException, a full resolution stack of where the error occured in the dependancy chain is displayed in the error.

## Extensibility

As with any framework, it is not complete unless it is open to extension. The functionality behind serveral aspects of this framework can be swapped out with custom implementations.

### IRegistrationSource

When registering new services, the registration will need to have a storage and retrieval mechanism when attempting to resolve new service instances.  

The IRegistrationSource requires a single instance throughout the lifetime of the ContainerRuntime. It can register single, or multiple services for a single type.

The IRegistrationSource has the following interface:

```c#
public interface IRegistrationSource
{
    void RegisterService(ServiceRegistration registration);

    void RegisterServices(Type serviceType, IEnumerable<ServiceRegistration> registrations);

    IEnumerable<ServiceRegistration> GetRegistrations(Type type);
}
```

And you can pass it directly to the ContainerBuilder like so:
```c#
var containerBuilder = new ContainerBuilder()
    .UseRegistrationSource(new CustomRegistrationSource());
```

Note: Setting a new registration source will overrie all previously registered services.

### IContainerBackend

To abstract away all internal functionality for this framework, a IContainerBackend interface was created to sperate it out from basic service management. This backend system can be overriden to allow the user to provide custom implementation of specific backend logic.

If only specific methods want to be overriden, then the user can inheric from the DefaultContainerBackend and override any of the desired method.

The IContainerBackend has the following interface:

```c#
public interface IContainerBackend
{
    IScopeContext CreateScopeContext();

    object CreateService(Type type, IEnumerable<object> services);

    object ActivateInstance(ServiceRegistration registration, DependencyChain chain, IContainerVisitor visitor);
}
```

The custom ContainerBackend can be passed in directly to the ContainerBuilder like so:
```c#
var containerBuilder = new ContainerBuilder()
    .UseBackend(new CustomContainerBackend());
```

Details of the above methods will be discussed in later sections.

### IScopeContext

When creating a new container to hold services and their corresponding instances, a ScopeContext will always be required.
Due to the highly coupled nature of the ContainerRuntime and the ContainerScope, the IScopeContext interface was created to abstract away the storage, retrieval, and disposal of service instances as they are created for each scope during the lifetime of the application.  

When a new runtime is built, a seperate runtime 'scope' is created to manage the instances of all services with a Singleton lifespan. Seperate scopes are also created upon the user's request through the containerRunime.CreateScope() method. These scopes are used to managed instances of Scoped services.  

When creating a new instance of the ScopeContext, the CreateScopeContext() method on the ContainerBackend can be overriden to instanciate new custom implementation of the IScopeContext interface.

The container scope will then either add or retrieve services from the IServiceContext interface as required and will dispose of it when itself is disposed later in the application.  

The ScopeContext has the following interfaces:

```c#
public interface IScopeContext : IDisposable
{
    void AddService(Type serviceType, object instance);
    object GetService(Type type);
}
```

### Service Activatation

When the registration has been found for a service and the correct scope has been assigned, the ActivateInstance() method is called on the activator.
It passes through the following:
* A service registration, to identify the details for that given service.
* A dependancy chain of where the service is at in relation to other dependancies.
* And an IServiceVisitor interface, to locate child services and activate factory methods.

This is where all of the constructor invocation and dependancy injection takes place.

### Service Creation

When all service instances have been instanciated for a specific type, the resulting service needs to be fully instanciated and correctly returned to the requested source. This method is required due to service types having multiple implementations.

A list of all resolved types are passed in. Based on the service type requested, the returning instance needs to be identified and potentially instanciated if required.

## Missing Features

Most features desired by a Dependancy Injection framework have been implemented. But here's some ideas for future improvements:

* Performance and concurency improvements.
* Support for invocation interceptors.


