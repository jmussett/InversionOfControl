using FluentAssertions;
using Xunit;

namespace InversionOfControl.UnitTests
{
    public class DelegateTests
    {
        [Fact]
        public void AddSingletonInstanceDelegateShouldReturnNewInstance()
        {
            var testType = new TestType();
            TestTypeFactory factory = () => testType;

            var runtime = new ContainerBuilder()
                .AddSingleton(factory)
                .BuildRuntime();

            var resolvedFactory1 = runtime.GetService<TestTypeFactory>();
            var resolvedFactory2 = runtime.GetService<TestTypeFactory>();

            resolvedFactory1.Should().BeSameAs(factory);
            resolvedFactory2.Should().BeSameAs(factory);
            resolvedFactory1().Should().BeSameAs(testType);
            resolvedFactory2().Should().BeSameAs(testType);
        }

        [Fact]
        public void AddSingletonFactoryDelegateShouldReturnNewInstance()
        {
            var testType = new TestType();

            var runtime = new ContainerBuilder()
                .AddSingleton<ITestType>(testType)
                .AddSingleton<TestTypeFactory>(sp => () => sp.GetService<ITestType>())
                .BuildRuntime();

            var factory1 = runtime.GetService<TestTypeFactory>();
            var factory2 = runtime.GetService<TestTypeFactory>();

            factory1.Should().BeSameAs(factory2);
            factory1().Should().BeSameAs(testType);
        }

        [Fact]
        public void AddSingletonFactoryDelegateShouldReturnNewInstanceWhenResolvedInConstructor()
        {
            var testType = new TestType();

            var runtime = new ContainerBuilder()
                .AddSingleton<ITestType>(testType)
                .AddSingleton<TestTypeFactory>(sp => () => sp.GetService<ITestType>())
                .AddTransient<DelegateClass>()
                .BuildRuntime();

            var delegateClass = runtime.GetService<DelegateClass>();

            delegateClass.TestType.Should().BeSameAs(testType);
        }

        public delegate ITestType TestTypeFactory();

        public class DelegateClass
        {
            public ITestType TestType { get; }

            public DelegateClass(TestTypeFactory factory)
            {
                TestType = factory();
            }
        }
    }
}
