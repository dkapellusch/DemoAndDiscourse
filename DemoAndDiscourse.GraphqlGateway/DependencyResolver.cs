using System;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;

namespace DemoAndDiscourse.GraphqlGateway
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Resolve<T>()
        {
            var service = _serviceProvider.GetService<T>();
            return service;
        }

        public object Resolve(Type type)
        {
            var service = _serviceProvider.GetService(type);
            return service;
        }
    }
}