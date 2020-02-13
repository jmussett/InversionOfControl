using FluentAssertions;
using System.Diagnostics;
using Xunit;

namespace InversionOfControl.UnitTests
{
    public class PerformanceTests
    {
        [Fact]
        public  static void ConstructViaFunc()
        {
            var runtime = new ContainerBuilder()
                .AddTransient<A>()
                .AddTransient<B>()
                .AddTransient<C>()
                .AddTransient<D>()
                .AddTransient<E>()
                .AddTransient<F>()
                .AddTransient<G>()
                .BuildRuntime();

            var timer = Stopwatch.StartNew();

            for (var i = 0; i <= 10000; i++)
            {
                runtime.GetService<A>();
                runtime.GetService<B>();
                runtime.GetService<C>();
                runtime.GetService<D>();
                runtime.GetService<E>();
                runtime.GetService<F>();
                runtime.GetService<G>();
            }

            timer.Stop();

            // Before constructor expressions: 450ms - 550ms
            // After constructor expressions: 150ms - 250ms

            timer.ElapsedMilliseconds.Should().BeLessThan(300);
        }
    }
}
