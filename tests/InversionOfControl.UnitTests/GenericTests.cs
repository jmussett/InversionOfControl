using FluentAssertions;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace InversionOfControl.UnitTests
{
    public class GenericTests
    {
        [Fact]
        public void AddTransientGenericShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddTransient<IGenericType<TestType>, GenericType<TestType>>()
                .BuildRuntime();

            var test1 = runtime.GetService<IGenericType<TestType>>();
            var test2 = runtime.GetService<IGenericType<TestType>>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().NotBeSameAs(test2);
        }

        [Fact]
        public void AddTransientGenericTypeDefinititionShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddTransient(typeof(IGenericType<>), typeof(GenericType<>))
                .BuildRuntime();

            var test1 = runtime.GetService<IGenericType<TestType>>();
            var test2 = runtime.GetService<IGenericType<TestType>>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().NotBeSameAs(test2);
        }

        [Fact]
        public void AddTransientNestedGenericTypeDefinititionShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddTransient(typeof(IGenericType<>), typeof(GenericType<>))
                .AddTransient<NestedGeneric>()
                .BuildRuntime();

            var test1 = runtime.GetService<NestedGeneric>();
            var test2 = runtime.GetService<NestedGeneric>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().NotBeSameAs(test2);

            test1.GenericType.Should().NotBeNull();
            test2.GenericType.Should().NotBeNull();
        }

        [Fact]
        public void AddTransientMultipleGenericTypeDefinititionShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddTransient(typeof(IMultipleGenericType<,>), typeof(MultipleGenericType<,>))
                .BuildRuntime();

            var test1 = runtime.GetService<IMultipleGenericType<string, string>>();
            var test2 = runtime.GetService<IMultipleGenericType<string, string>>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().NotBeSameAs(test2);
        }

        public interface IGenericType<T> { }

        public class GenericType<T> : IGenericType<T> { }

        public interface IMultipleGenericType<T, R> { }

        public class MultipleGenericType<T, R> : IMultipleGenericType<T, R> { }

        public class NestedGeneric
        {
            public IGenericType<TestType> GenericType { get; }

            public NestedGeneric(IGenericType<TestType> genericType)
            {
                GenericType = genericType;
            }
        }

    }
}
