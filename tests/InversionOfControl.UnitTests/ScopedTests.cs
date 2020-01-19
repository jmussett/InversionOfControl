using FluentAssertions;
using System;
using Xunit;

namespace InversionOfControl.UnitTests
{
    public class ScopedTests
    {
        [Fact]
        public void AddScopedShouldReturnNewInstanceWithSameScope()
        {
            var scope = new ContainerBuilder()
                .AddScoped<TestType>()
                .BuildRuntime()
                .CreateScope();

            var test1 = scope.GetService<TestType>();
            var test2 = scope.GetService<TestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeSameAs(test2);
        }

        [Fact]
        public void AddScopedFactoryShouldReturnNewInstanceWithSameScope()
        {
            var scope = new ContainerBuilder()
                .AddScoped(r => new TestType { TestString = "Test" })
                .BuildRuntime()
                .CreateScope();

            var test1 = scope.GetService<TestType>();
            var test2 = scope.GetService<TestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeSameAs(test2);

            test1.TestString.Should().Be("Test");
        }

        [Fact]
        public void AddScopedConcreteShouldReturnNewInstanceWithSameScope()
        {
            var scope = new ContainerBuilder()
                .AddScoped<ITestType, TestType>()
                .BuildRuntime()
                .CreateScope();

            var test1 = scope.GetService<ITestType>();
            var test2 = scope.GetService<ITestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeOfType<TestType>();
            test2.Should().BeOfType<TestType>();

            test1.Should().BeSameAs(test2);
        }

        [Fact]
        public void AddScopedConcreteFactoryShouldReturnNewInstanceWithSameScope()
        {
            var scope = new ContainerBuilder()
                .AddScoped<ITestType>(r => new TestType { TestString = "Test" })
                .BuildRuntime()
                .CreateScope();

            var test1 = scope.GetService<ITestType>();
            var test2 = scope.GetService<ITestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeOfType<TestType>();
            test2.Should().BeOfType<TestType>();

            test1.Should().BeSameAs(test2);

            (test1 as TestType).TestString.Should().Be("Test");
        }

        [Fact]
        public void AddScopedShouldReturnNewInstanceWithDifferentScopes()
        {
            var runtime = new ContainerBuilder()
                .AddScoped<TestType>()
                .BuildRuntime();

            var test1 = runtime.CreateScope().GetService<TestType>();
            var test2 = runtime.CreateScope().GetService<TestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().NotBeSameAs(test2);
        }

        [Fact]
        public void AddScopedFactoryShouldReturnNewInstanceWithDifferentScopes()
        {
            var runtime = new ContainerBuilder()
                .AddScoped(r => new TestType { TestString = "Test" })
                .BuildRuntime();

            var test1 = runtime.CreateScope().GetService<TestType>();
            var test2 = runtime.CreateScope().GetService<TestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().NotBeSameAs(test2);

            test1.TestString.Should().Be("Test");
            test2.TestString.Should().Be("Test");
        }

        [Fact]
        public void AddScopedConcreteShouldReturnNewInstanceWithDifferentScopes()
        {
            var runtime = new ContainerBuilder()
                .AddScoped<ITestType, TestType>()
                .BuildRuntime();

            var test1 = runtime.CreateScope().GetService<ITestType>();
            var test2 = runtime.CreateScope().GetService<ITestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeOfType<TestType>();
            test2.Should().BeOfType<TestType>();

            test1.Should().NotBeSameAs(test2);
        }

        [Fact]
        public void AddScopedConcreteFactoryShouldReturnNewInstanceWithDifferentScopes()
        {
            var runtime = new ContainerBuilder()
                .AddScoped<ITestType>(r => new TestType { TestString = "Test" })
                .BuildRuntime();

            var test1 = runtime.CreateScope().GetService<ITestType>();
            var test2 = runtime.CreateScope().GetService<ITestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeOfType<TestType>();
            test2.Should().BeOfType<TestType>();

            test1.Should().NotBeSameAs(test2);

            (test1 as TestType).TestString.Should().Be("Test");
            (test2 as TestType).TestString.Should().Be("Test");
        }

        [Fact]
        public void DisposeScopeShouldDisposeInstances()
        {
            var scope = new ContainerBuilder()
                .AddScoped<DisposableClass>()
                .BuildRuntime()
                .CreateScope();

            var test = scope.GetService<DisposableClass>();

            test.Should().NotBeNull();
            test.Disposed.Should().BeFalse();

            scope.Dispose();

            test.Disposed.Should().BeTrue();
        }

        private class DisposableClass : IDisposable
        {
            public bool Disposed = false;
            public void Dispose() => Disposed = true;
        }
    }
}
