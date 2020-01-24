using System;
using System.Collections.Generic;

namespace InversionOfControl
{
    public static class ContainerExtensions
    {
        public static TService GetService<TService>(this IContainer container)
            => (TService)container.GetService(typeof(TService));

        public static TService GetRequiredService<TService>(this IContainer container)
        {
            var service = container.GetService<TService>();

            if (service == null)
                throw new MissingServiceException(typeof(TService));

            return service;
        }

        public static IEnumerable<object> GetServices(this IContainer container, Type type)
        {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(type);
            return (IEnumerable<object>) container.GetService(enumerableType);
        }

        public static IEnumerable<TService> GetServices<TService>(this IContainer container)
            => (IEnumerable<TService>)container.GetService(typeof(IEnumerable<TService>));
    }
}
