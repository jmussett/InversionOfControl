# Inversion Of Control
This is my attempt at implementing a fully functioning Dependency Injection framework from scratch.
It's inspired by [Microsoft's ASP .Net Core Dependancy Injection Framework](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)

## The Basics
This library manages the dependancies of classes (or services) automatically by allowing the user to register them to a container at the start of the application.
You can register services to the container and build your runtime like so:

```c#
var containerRuntime = new ContainerBuilder()
    .AddTransient<SomeClass>()
    .BuildRuntime();
```

Resolving a service is strait forward:
```c#
var service = containerRuntime.GetService<SomeClass>();
```

Sometimes, you'll want to create a seperate scope to limit the lifespan of scoped services you've registered:
```c#
var containerRuntime = new ContainerBuilder()
    .AddTransient<ScopedClass>()
    .BuildRuntime();

using(var scope = containerRuntime.CreateScope())
{
    var scopedService = scope.GetService<ScopedClass>();
}
```
If the scoped service you have registered implements IDisposable, the service will automatically be disposed of when it exists the scope.

## Registering Services

When registering a service, you can specify it's lifespan to determine when it instantiates new instances. This is defined it the method name.
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

You can specify interfaces with coresponding concrete types by adding an additional generic type parameter:
```c#
    .AddTransient<ISomeInterface, SomeClass>()
```

You can specify the instance of the class when registering it as a singleton:
```c#
    .AddSingleton<ISomeInterface>(new SomeClass())
```

And you can specify factory methods to instantiate the instance with a callback:
```c#
    .AddSingleton<ISomeInterface>(runtime => new SomeClass(runtime.GetService<SomeDependency>()))
```
The container runtime passed in above allows you to resolve other dependancies for the class you want to instantiate, if required.  

The framework also supports registering unbound types, this is beneficial when registering generic type definitions without specified generic type parameters:
```c#
    .AddTransient(typeof(ISomeGenericInterface<>), typeof(SomeGenericClass<>))
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
As you can see, ServiceA depends on ServiceB being passed through to it's constructor, and ServiceB depends on ServiceC.
This frameworks resolves those dependancies automatically without having to instantiate them manually each time.
As long as they are all registered it will work as expected.  

In C#, classes support multiple constructors. With this framwork, I decided to do the same.
Currently, the framework looks for the constructor with the most amount of parameters. If it fails to invoke that constructor due to missing dependancies, it will attempt to invoke the next constructor, and so on...

## Exception Handling

An important aspect to this framework is the way it manages exceptions.
There are currently 4 different exceptions that it currently throws:
* MissingServiceException - thrown when the service that was requested was not registered in the container.
* MissingDependencyException - thrown when a dependancy for a service was not registered in the container.
* MissingConstructorException - thrown when no public constructors are available for a requested service.
* CircularDependencyException - thrown when a circular dependancy was found in the dependancy chain.

With MissingDependencyException and CircularDependencyException, a full resolution stack of where the error occured in the dependancy chain is displayed in the error.

## Extensibility

As with any framework, it is not complete unless it is open to extension. The functionality behind serveral aspects of this framework can be swapped out with your own implementation, at your own discretion of course.

### IServiceContextFactory and IServiceContext

When creating a new container to hold services and their corresponding instances, a ServiceContext will always be required.
Due to the highly coupled nature of the ContainerRuntime and the ContainerScope, I decided to create the IServiceContext interface to abstract away the storage, retrieval, and disposal of service instances as they are created during the lifetime of the application.  

When a new runtime is built, a seperate runtime 'scope' is created to manage the instances of all services with a Singleton lifespan. Seperate scopes are also created upon the user's request through the containerRunime.CreateScope() method. These scopes are used to managed instances of Scoped services.  

Therefor, each ContainerScope needs a newly instanciated ServiceContext, allowing the scopes to have clean slates to work with. This is where the ServiceContextFactory comes in.  

The ServiceContextFactory and ServiceContext have the following interfaces:

```c#
public interface IServiceContext : IDisposable
{
    void AddService(Type serviceType, object instance);
    object GetService(Type type);
}

public interface IServiceContextFactory
{
    IServiceContext CreateContext();
}
```

Whenever a new scope is created, the CreateContext() method is called instanciating a new instance of the ServiceContext.  

The container scope will then either add or retrieve services from the IServiceContext interface as required and will dispose of it when itself is disposed later in the application.  

You can implement these interfaces yourself by passing the IServiceContextFactory directly to the ContainerBuilder like so:

```c#
var containerBuilder = new ContainerBuilder(new CustomServiceContextFactory());
```

### IRegistrationContext

When registering new services, the registration will need to have a storage and retrieval mechanism when attempting to resolve new service instances.  

The IRegistrationContext does just that, exactly like what the IServiceContext does when it comes to service instances. Only difference is it requires a single instance throughout the lifetime of the ContainerRuntime.  

The IRegistrationContext has the following interfaces:

```c#
public interface IRegistrationContext
{
    void RegisterService(ServiceRegistration registration);
    ServiceRegistration GetRegistration(Type type);
}
```

And you can pass it directly to the ContainerBuilder like so:

```c#
var containerBuilder = new ContainerBuilder(new CustomRegistrationContext());
```

### IServiceActivator

And last but not least, the IServiceActivator. This interface is where all of the constructor invocation and dependancy injection takes place.  

When the registration has been found for a service and the correct scope has been assigned, the ActivateInstance() method is called on the activator.
It passes through the following:
* A service registration, allowing you to identify the details for that given service.
* A dependancy chain of where the service is at in relation to other dependancies.
* And an IServiceVisitor interface, allowing you to locate child services and activate factory methods.

Below is the interface for the ServiceActivator, the rest of the implementation is down to you:

```c#
public interface IServiceActivator
{
    object ActivateInstance(ServiceRegistration registration, DependencyChain chain, IServiceVisitor visitor);
}
```

Like all the other extensable interfaces, you can pass it to the ContainerBuilder like so:

```c#
var containerBuilder = new ContainerBuilder(new CustomServiceActivator());
```

## Missing Features

Most features I've wanted from a Dependancy Injection framework have been implemented. But here's a few extras I intend to look at in the future, when I have time:

* Support for multiple implementations of a single service types, allowing the user to resolve an IEnumerable of services.
* Performance and Concurency improvements.
* Support for invocation interceptors.


