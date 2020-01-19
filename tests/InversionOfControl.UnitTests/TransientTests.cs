using FluentAssertions;
using Xunit;

namespace InversionOfControl.UnitTests
{
    public class TransientTests
    {
        [Fact]
        public void AddTransientShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddTransient<TestType>()
                .BuildRuntime();

            var test1 = runtime.GetService<TestType>();
            var test2 = runtime.GetService<TestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().NotBeSameAs(test2);
        }

        [Fact]
        public void AddTransientFactoryShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddTransient(r => new TestType { TestString = "Test" })
                .BuildRuntime();

            var test1 = runtime.GetService<TestType>();
            var test2 = runtime.GetService<TestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().NotBeSameAs(test2);

            test1.TestString.Should().Be("Test");
            test2.TestString.Should().Be("Test");
        }

        [Fact]
        public void AddTransientConcreteShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddTransient<ITestType, TestType>()
                .BuildRuntime();

            var test1 = runtime.GetService<ITestType>();
            var test2 = runtime.GetService<ITestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeOfType<TestType>();
            test2.Should().BeOfType<TestType>();

            test1.Should().NotBeSameAs(test2);
        }

        [Fact]
        public void AddTransientConcreteFactoryShouldReturnNewInstance()
        {
            var runtime = new ContainerBuilder()
                .AddTransient<ITestType>(r => new TestType { TestString = "Test" })
                .BuildRuntime();

            var test1 = runtime.GetService<ITestType>();
            var test2 = runtime.GetService<ITestType>();

            test1.Should().NotBeNull();
            test2.Should().NotBeNull();

            test1.Should().BeOfType<TestType>();
            test2.Should().BeOfType<TestType>();

            test1.Should().NotBeSameAs(test2);

            (test1 as TestType).TestString.Should().Be("Test");
            (test2 as TestType).TestString.Should().Be("Test");
        }
    }
}
