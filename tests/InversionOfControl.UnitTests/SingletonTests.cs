using FluentAssertions;
using Xunit;

namespace InversionOfControl.UnitTests
{
    public class SingletonTests
    {
        [Fact]
        public void AddSingletonShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddSingleton<TestType>()
                .BuildRuntime();

            var test1 = runtime.GetService<TestType>();
            var test2 = runtime.GetService<TestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeSameAs(test2);
        }

        [Fact]
        public void AddSingletonInstanceShouldReturnNewInstance()
        {
            var instance = new TestType();

            var runtime = new ContainerBuilder()
                .AddSingleton(instance)
                .BuildRuntime();

            var test1 = runtime.GetService<TestType>();
            var test2 = runtime.GetService<TestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeSameAs(instance);
            test2.Should().BeSameAs(instance);
        }

        [Fact]
        public void AddSingletonFactoryShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddSingleton(r => new TestType { TestString = "Test" })
                .BuildRuntime();

            var test1 = runtime.GetService<TestType>();
            var test2 = runtime.GetService<TestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeSameAs(test2);

            test1.TestString.Should().Be("Test");
        }

        [Fact]
        public void AddSingletonConcreteShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddSingleton<ITestType, TestType>()
                .BuildRuntime();

            var test1 = runtime.GetService<ITestType>();
            var test2 = runtime.GetService<ITestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeOfType<TestType>();
            test2.Should().BeOfType<TestType>();

            test1.Should().BeSameAs(test2);
        }

        [Fact]
        public void AddSingletonConcreteInstanceShouldReturnNewInstance()
        {
            var instance = new TestType();

            var runtime = new ContainerBuilder()
                .AddSingleton<ITestType, TestType>(instance)
                .BuildRuntime();

            var test1 = runtime.GetService<ITestType>();
            var test2 = runtime.GetService<ITestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeOfType<TestType>();
            test2.Should().BeOfType<TestType>();

            test1.Should().BeSameAs(instance);
            test1.Should().BeSameAs(instance);
        }

        [Fact]
        public void AddSingletonConcreteFactoryShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddSingleton<ITestType>(r => new TestType { TestString = "Test" })
                .BuildRuntime();

            var test1 = runtime.GetService<ITestType>();
            var test2 = runtime.GetService<ITestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeOfType<TestType>();
            test2.Should().BeOfType<TestType>();

            test1.Should().BeSameAs(test2);

            (test1 as TestType).TestString.Should().Be("Test");
        }
    }
}
