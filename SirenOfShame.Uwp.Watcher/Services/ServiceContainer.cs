using System;
using System.Collections.Generic;

namespace SirenOfShame.Uwp.Watcher.Services
{
    /// <summary>
    /// A simple service container implementation, singleton only
    /// </summary>
    public static class ServiceContainer
    {
        static readonly Dictionary<Type, Func<object>> Types = new Dictionary<Type, Func<object>>();
        static readonly Dictionary<Type, Lazy<object>> Services = new Dictionary<Type, Lazy<object>>();
        static readonly Stack<Dictionary<Type, object>> ScopedServices = new Stack<Dictionary<Type, object>>();

        #region Register methods Types
        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="service">Service to register.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void RegisterType<T>(T service)
        {
            Types[typeof(T)] = () => service;
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void RegisterType<T>()
            where T : new()
        {
            Types[typeof(T)] = () => new T();
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="function">Func to register.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void RegisterType<T>(Func<object> function)
        {
            Types[typeof(T)] = function;
        }
        #endregion

        /// <summary>
        /// Register the specified service with an instance
        /// </summary>
        /// <param name="service">Service to register.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Register<T>(T service)
        {
            Services[typeof(T)] = new Lazy<object>(() => service);
        }

        /// <summary>
        /// Register the specified service for a class with a default constructor
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Register<T>() where T : new()
        {
            Services[typeof(T)] = new Lazy<object>(() => new T());
        }

        /// <summary>
        /// Register the specified service with a callback to be invoked when requested
        /// </summary>
        /// <param name="function">Function to register</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Register<T>(Func<T> function)
        {
            Services[typeof(T)] = new Lazy<object>(() => function());
        }

        /// <summary>
        /// Register the specified service with an instance
        /// </summary>
        /// <param name="type">Type object</param>
        /// <param name="service">Service object</param>
        public static void Register(Type type, object service)
        {
            Services[type] = new Lazy<object>(() => service);
        }

        /// <summary>
        /// Register the specified service with a callback to be invoked when requested
        /// </summary>
        /// <param name="type">Type object</param>
        /// <param name="function">Function object</param>
        public static void Register(Type type, Func<object> function)
        {
            Services[type] = new Lazy<object>(function);
        }

        /// <summary>
        /// Register the specified service with an instance that is scoped
        /// </summary>
        /// <param name="service">Service object</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void RegisterScoped<T>(T service)
        {
            Dictionary<Type, object> services;
            if (ScopedServices.Count == 0)
            {
                services = new Dictionary<Type, object>();
                ScopedServices.Push(services);
            }
            else
            {
                services = ScopedServices.Peek();
            }

            services[typeof(T)] = service;
        }

        /// <summary>
        /// Resolve this instance.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        /// Resolve the specified type.
        /// </summary>
        /// <param name="type">Object type</param>
        public static object Resolve(Type type)
        {
            // Scoped services
            if (ScopedServices.Count > 0)
            {
                var services = ScopedServices.Peek();

                object service;
                if (services.TryGetValue(type, out service))
                {
                    return service;
                }
            }

            // Non-scoped services
            {
                Lazy<object> service;
                if (Services.TryGetValue(type, out service))
                {
                    return service.Value;
                }
                throw new KeyNotFoundException(string.Format("Service not found for type '{0}'", type));
            }
        }

        /// <summary>
        /// Resolve the specified type.
        /// </summary>
        /// <returns>Object type.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T ResolveType<T>()
        {
            Func<object> type;
            if (Types.TryGetValue(typeof(T), out type))
            {
                return (T)type();
            }
            else
            {
                throw new KeyNotFoundException(string.Format("Type not found for type '{0}'. See ServiceContainer.ResolveType.", typeof(T)));
            }
        }

        /// <summary>
        /// Adds a "scope" which is a way to register a service on a stack to be popped off at a later time
        /// </summary>
        public static void AddScope()
        {
            ScopedServices.Push(new Dictionary<Type, object>());
        }

        /// <summary>
        /// Removes the current "scope" which pops off some local services
        /// </summary>
        public static void RemoveScope()
        {
            if (ScopedServices.Count > 0)
            {
                ScopedServices.Pop();
            }
        }

        /// <summary>
        /// Mainly for testing, clears the entire container
        /// </summary>
        public static void Clear()
        {
            Services.Clear();
            ScopedServices.Clear();
        }
    }
}
