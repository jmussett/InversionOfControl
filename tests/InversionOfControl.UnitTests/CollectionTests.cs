using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace InversionOfControl.UnitTests
{
    public class CollectionTests
    {
        [Fact]
        public void AddMultipleTypesShouldResolveEnumerable()
        {
            var runtime = new ContainerBuilder()
                .AddTransient<ICollectionType, CollectionType1>()
                .AddTransient<ICollectionType, CollectionType2>()
                .BuildRuntime();

            var test1 = runtime.GetServices<ICollectionType>();

            test1.Should().NotBeNull();

            test1.Should().HaveCount(2);
        }

        [Fact]
        public void AddMultipleTypesShouldResolveCollection()
        {
            var runtime = new ContainerBuilder()
                .AddTransient<ICollectionType, CollectionType1>()
                .AddTransient<ICollectionType, CollectionType2>()
                .BuildRuntime();

            var test1 = runtime.GetService<ICollection<ICollectionType>>();

            test1.Should().NotBeNull();

            test1.Should().HaveCount(2);
        }

        [Fact]
        public void AddMultipleTypesShouldResolveList()
        {
            var runtime = new ContainerBuilder()
                .AddTransient<ICollectionType, CollectionType1>()
                .AddTransient<ICollectionType, CollectionType2>()
                .BuildRuntime();

            var test1 = runtime.GetService<IList<ICollectionType>>();

            test1.Should().NotBeNull();

            test1.Should().HaveCount(2);
        }

        [Fact]
        public void AddMultipleTypesShouldResolveNestedEnumerable()
        {
            var runtime = new ContainerBuilder()
                .AddTransient<NestedEnumerable>()
                .AddTransient<ICollectionType, CollectionType1>()
                .AddTransient<ICollectionType, CollectionType2>()
                .BuildRuntime();

            var test1 = runtime.GetService<NestedEnumerable>();

            test1.Should().NotBeNull();

            test1.Children.Should().HaveCount(2);
        }

        public interface ICollectionType { }

        public class CollectionType1 : ICollectionType { }

        public class CollectionType2 : ICollectionType { }

        public class NestedEnumerable
        {
            public IEnumerable<ICollectionType> Children { get; }

            public NestedEnumerable(IEnumerable<ICollectionType> children)
            {
                Children = children;
            }
        }
    }
}
