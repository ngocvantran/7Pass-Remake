using System;
using System.Collections.Generic;
using AutoMapper;

namespace SevenPass
{
    internal static class ExtensionMethods
    {
        public static IProjection Project<T>(
            this IEnumerable<T> source, IMappingEngine engine)
        {
            return new Source<T>(engine, source);
        }

        public interface IProjection
        {
            IEnumerable<TTarget> To<TTarget>();
        }

        private class Source<TSource> : IProjection
        {
            private readonly IMappingEngine _engine;
            private readonly IEnumerable<TSource> _source;

            public Source(IMappingEngine engine, IEnumerable<TSource> source)
            {
                if (engine == null) throw new ArgumentNullException("engine");
                if (source == null) throw new ArgumentNullException("source");

                _engine = engine;
                _source = source;
            }

            public IEnumerable<TTarget> To<TTarget>()
            {
                return _engine.Map<IEnumerable<TSource>,
                    IEnumerable<TTarget>>(_source);
            }
        }
    }
}