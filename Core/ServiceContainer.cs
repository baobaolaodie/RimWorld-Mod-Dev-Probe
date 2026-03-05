using System;
using System.Collections.Generic;

namespace RimWorldModDevProbe.Core
{
    public class ServiceContainer
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Func<object>> _factories = new Dictionary<Type, Func<object>>();
        private readonly Dictionary<(Type type, string name), object> _namedServices = new Dictionary<(Type, string), object>();

        public void RegisterSingleton<TInterface, TImplementation>(TImplementation instance)
            where TImplementation : TInterface
        {
            _services[typeof(TInterface)] = instance;
        }

        public void RegisterSingleton<T>(T instance)
        {
            _services[typeof(T)] = instance;
        }

        public void RegisterSingleton<T>(string name, T instance)
        {
            _namedServices[(typeof(T), name)] = instance;
        }

        public void RegisterFactory<T>(Func<T> factory)
        {
            _factories[typeof(T)] = () => factory();
        }

        public void RegisterFactory<TInterface, TImplementation>(Func<TImplementation> factory)
            where TImplementation : TInterface
        {
            _factories[typeof(TInterface)] = () => factory();
        }

        public T Resolve<T>()
        {
            var type = typeof(T);

            if (_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }

            if (_factories.TryGetValue(type, out var factory))
            {
                return (T)factory();
            }

            throw new InvalidOperationException($"Service of type {type.Name} is not registered.");
        }

        public T Resolve<T>(string name)
        {
            var key = (typeof(T), name);
            if (_namedServices.TryGetValue(key, out var service))
            {
                return (T)service;
            }

            throw new InvalidOperationException($"Named service '{name}' of type {typeof(T).Name} is not registered.");
        }

        public bool TryResolve<T>(out T service)
        {
            service = default;
            var type = typeof(T);

            if (_services.TryGetValue(type, out var instance))
            {
                service = (T)instance;
                return true;
            }

            if (_factories.TryGetValue(type, out var factory))
            {
                service = (T)factory();
                return true;
            }

            return false;
        }

        public bool TryResolve<T>(string name, out T service)
        {
            service = default;
            var key = (typeof(T), name);
            
            if (_namedServices.TryGetValue(key, out var instance))
            {
                service = (T)instance;
                return true;
            }

            return false;
        }

        public bool IsRegistered<T>()
        {
            return _services.ContainsKey(typeof(T)) || _factories.ContainsKey(typeof(T));
        }

        public bool IsRegistered<T>(string name)
        {
            return _namedServices.ContainsKey((typeof(T), name));
        }

        public IEnumerable<string> GetRegisteredNames<T>()
        {
            var type = typeof(T);
            foreach (var kvp in _namedServices)
            {
                if (kvp.Key.type == type)
                {
                    yield return kvp.Key.name;
                }
            }
        }

        public void Clear()
        {
            _services.Clear();
            _factories.Clear();
            _namedServices.Clear();
        }
    }
}
