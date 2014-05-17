using System;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;

namespace SevenPass
{
    public static class ContainerExtensions
    {
        public static AssemblyActions AssemblyContainingType<T>(this SimpleContainer container)
        {
            var assembly = typeof(T).GetTypeInfo().Assembly;
            return new AssemblyActions(container, assembly);
        }

        public class AssemblyActions
        {
            private readonly Assembly _assembly;
            private readonly SimpleContainer _container;

            public AssemblyActions(SimpleContainer container, Assembly assembly)
            {
                if (container == null) throw new ArgumentNullException("container");
                if (assembly == null) throw new ArgumentNullException("assembly");

                _assembly = assembly;
                _container = container;
            }

            public AssemblyActions RegisterInstances<T>()
            {
                var service = typeof(T);
                var serviceInfo = service.GetTypeInfo();

                _assembly.DefinedTypes
                    .Where(x => !x.IsAbstract && !x.IsInterface &&
                        serviceInfo.IsAssignableFrom(x))
                    .Select(x => x.AsType())
                    .Apply(x => _container.RegisterPerRequest(service, null, x));

                return this;
            }

            public AssemblyActions RegisterViewModels()
            {
                _assembly.DefinedTypes
                    .Where(x => x.Name.EndsWith("ViewModel", StringComparison.Ordinal))
                    .Where(x => !x.IsAbstract && !x.IsInterface)
                    .Select(x => x.AsType())
                    .Apply(x => _container.RegisterPerRequest(x, null, x));

                return this;
            }
        }
    }
}