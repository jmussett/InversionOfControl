using FluentAssertions;
using System;
using Xunit;

namespace InversionOfControl.UnitTests
{
    public class ExceptionTests
    {
        [Fact]
        public void CircularDependanciesShouldThrowCircularDependencyException()
        {
            var runtime = new ContainerBuilder()
                .AddSingleton<CircularDependencyA>()
                .AddSingleton<CircularDependencyB>()
                .BuildRuntime();

            runtime.Invoking(r => r.GetService<CircularDependencyA>())
                .Should()
                .Throw<CircularDependencyException>()
                .WithMessage($"A circular dependency was found when attempting to instanciate type '{typeof(CircularDependencyA).FullName}'.{Environment.NewLine}" +
                    $"Resolution Stack:{Environment.NewLine}" +
                    // Since it's a circular dependency, CircularDependencyA is displayed twice
                    $"Type: {typeof(CircularDependencyA).FullName}{Environment.NewLine}" +
                    $"Type: {typeof(CircularDependencyB).FullName}{Environment.NewLine}" +
                    $"Type: {typeof(CircularDependencyA).FullName}");
        }

        [Fact]
        public void MissingDependanciesShouldThrowMissingDependencyException()
        {
            var runtime = new ContainerBuilder()
                .AddSingleton<A>()
                .AddSingleton<B>()
                .AddSingleton<C>()
                .AddSingleton<D>()
                .AddSingleton<E>()
                .AddSingleton<F>()
                // G is the missing dependency
                .BuildRuntime();

            runtime.Invoking(r => r.GetService<A>())
                .Should()
                .Throw<MissingDependencyException>()
                .WithMessage(
                    $"Unable to resolve dependency '{typeof(G).FullName}' for type '{typeof(C).FullName}'.{Environment.NewLine}" +
                    $"Resolution Stack:{Environment.NewLine}" +
                    // C depends on G
                    $"Type: {typeof(C).FullName}{Environment.NewLine}" +
                    // A depends on C
                    $"Type: {typeof(A).FullName}");
        }

        [Fact]
        public void MissingConstructorShouldThrowMissingServiceException()
        {
            var runtime = new ContainerBuilder()
                .BuildRuntime();

            runtime.Invoking(r => r.GetRequiredService<A>())
                .Should()
                .Throw<MissingServiceException>()
                .WithMessage($"Type not registered for service '{typeof(A).FullName}'.");
        }

        [Fact]
        public void MissingServiceShouldThrowMissingServiceException()
        {
            var runtime = new ContainerBuilder()
                .AddSingleton<MissingConstructor>()
                .BuildRuntime();

            runtime.Invoking(r => r.GetService<MissingConstructor>())
                .Should()
                .Throw<MissingConstructorException>()
                .WithMessage($"Unable to locate a public constructor for type '{typeof(MissingConstructor).FullName}'.");
        }

        public class MissingConstructor
        {
            private MissingConstructor() { }
        }

        public class CircularDependencyA
        {
            public CircularDependencyA(CircularDependencyB b) { }
        }

        public class CircularDependencyB
        {
            public CircularDependencyB(CircularDependencyA a) { }
        }
    }
}
