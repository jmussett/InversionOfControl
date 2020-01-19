using System;
using System.Collections.Generic;

namespace InversionOfControl
{
    public class TypeMap<T>
    {
        private readonly IDictionary<Type, List<T>> _map;

        public TypeMap() => _map = new Dictionary<Type, List<T>>();

        public void Add(Type type, T value)
        {
            // Service types can have multiple concrete types.
            // If the list for the specificied service type does not exist, we need to add it explicitly.
            if (_map.ContainsKey(type))
                _map.Add(type, new List<T> { value });
            else
                _map[type].Add(value);
        }

        public IEnumerable<T> Get(Type type)
        {
            if (!_map.TryGetValue(type, out var values))
                return new List<T>();

            return values;
        }

        public bool TryGet(Type type, out IEnumerable<T> values)
        {
            var response = _map.TryGetValue(type, out var mapValues);
            values = mapValues;
            return response;
        }
    }
}
