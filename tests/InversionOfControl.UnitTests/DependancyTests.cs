using FluentAssertions;
using Xunit;

namespace InversionOfControl.UnitTests
{
    public class DependencyTests
    {
        [Fact]
        public void ServicesShouldInstanciateWith3LevelsOfDependancies()
        {
            var runtime = new ContainerBuilder()
                .AddSingleton<A>()
                .AddSingleton<B>()
                .AddSingleton<C>()
                .AddSingleton<D>()
                .AddSingleton<E>()
                .AddSingleton<F>()
                .AddSingleton<G>()
                .BuildRuntime();

            var a = runtime.GetService<A>();
            var b = runtime.GetService<B>();
            var c = runtime.GetService<C>();
            var d = runtime.GetService<D>();
            var e = runtime.GetService<E>();
            var f = runtime.GetService<F>();
            var g = runtime.GetService<G>();

            a.Should().NotBeNull();
            a.B.Should().BeSameAs(b);
            a.C.Should().BeSameAs(c);
            a.B.D.Should().BeSameAs(d);
            a.B.E.Should().BeSameAs(e);
            a.C.F.Should().BeSameAs(f);
            a.C.G.Should().BeSameAs(g);
        }

        [Fact]
        public void ServicesShouldActivateWithLargestConstructor()
        {
            var runtime = new ContainerBuilder()
                .AddTransient(r => new TestType() { TestString = "Test" } )
                .AddTransient<MultipleConstructors>()
                .BuildRuntime();

            var multipleConstructors = runtime.GetService<MultipleConstructors>();

            multipleConstructors.Should().NotBeNull();
            multipleConstructors.TestString.Should().Be("Test");
        }

        [Fact]
        public void ServicesShouldActivateWithSecondLargestConstructorIfDependanciesNotFound()
        {
            var runtime = new ContainerBuilder()
                .AddTransient<MultipleConstructors>()
                .BuildRuntime();

            var multipleConstructors = runtime.GetService<MultipleConstructors>();

            multipleConstructors.Should().NotBeNull();
            multipleConstructors.TestString.Should().Be("DefaultConstructor");
        }

        public class MultipleConstructors
        {
            public string TestString { get; set; }

            public MultipleConstructors(TestType testType)
            {
                TestString = testType.TestString;
            }

            public MultipleConstructors()
            {
                TestString = "DefaultConstructor";
            }
        }
    }
}
