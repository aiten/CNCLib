/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace Framework.Dependency
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Abstraction;

    public static class DependencyContainerExtensions
    {
        /// <summary>
        /// Registers the given object for the interface. 
        /// </summary>
        /// <typeparam name="TInterface">Interface that can be later resolved.</typeparam>
        /// <param name="container">Dependency container.</param>
        /// <param name="obj">Object that should be returned for Resolve&lt;TInterface&gt;() calls.</param>
        /// <returns>This instance.</returns>
        public static IDependencyContainer RegisterInstance<TInterface>(this IDependencyContainer container, TInterface obj)
        {
            container.RegisterInstance(typeof(TInterface), obj);
            return container;
        }

        #region perInstance

        /// <summary>
        /// Registers a type for the given interface.
        /// </summary>
        /// <param name="container">Dependency container.</param>
        /// <typeparam name="TInterface">Interface that can be later resolved.</typeparam>
        /// <typeparam name="TType">Type that implements interface. On Resolve&lt;TInterface&gt;() calls a new instance is returned every time.</typeparam>
        /// <returns>This instance.</returns>
        public static IDependencyContainer RegisterType<TInterface, TType>(this IDependencyContainer container) where TType : TInterface
        {
            container.RegisterType(typeof(TInterface), typeof(TType));
            return container;
        }

        /// <summary>
        /// Registers a type for the given interface.
        /// </summary>
        /// <param name="container">Dependency container.</param>
        /// <returns>This instance.</returns>
        /// <typeparam name="TType">Type that implements interface.</typeparam>
        public static IDependencyContainer RegisterType<TType>(this IDependencyContainer container)
        {
            container.RegisterType(typeof(TType), typeof(TType));
            return container;
        }

        /// <summary>
        /// Registers public and internal types of the given assemblies with the unity container. This is necessary
        /// to workaround the internals visible to hacks in the code base.
        /// </summary>
        /// <param name="container">Dependency container.</param>
        /// <param name="assemblies">List of assemblies in which all types should be registered with their interfaces. 
        /// This includes internal types. </param>
        /// <returns>This instance.</returns>
        public static IDependencyContainer RegisterTypesIncludingInternals(this IDependencyContainer container, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
                {
                    string interfaceName = "I" + type.Name;
                    var    interfaceType = type.GetInterface(interfaceName);
                    if (interfaceType != null)
                    {
                        container.RegisterType(interfaceType, type);
                    }
                }
            }

            return container;
        }

        public static IDependencyContainer RegisterTypesByName(this IDependencyContainer container, Func<string, bool> checkName, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
                {
                    if (checkName(type.Name))
                    {
                        container.RegisterType(type, type);
                    }
                }
            }

            return container;
        }

        #endregion

        #region perScope

        /// <summary>
        /// Registers a type for the given interface.
        /// </summary>
        /// <typeparam name="TInterface">Interface that can be later resolved.</typeparam>
        /// <typeparam name="TType">Type that implements interface. On Resolve&lt;TInterface&gt;() calls a new instance is returned every time.</typeparam>
        /// <param name="container">Dependency container.</param>
        /// <returns>This instance.</returns>
        public static IDependencyContainer RegisterTypeScoped<TInterface, TType>(this IDependencyContainer container) where TType : TInterface
        {
            container.RegisterTypeScoped(typeof(TInterface), typeof(TType));
            return container;
        }

        /// <summary>
        /// Registers a type for the given interface.
        /// </summary>
        /// <param name="container">Dependency container.</param>
        /// <returns>This instance.</returns>
        /// <typeparam name="TType">Type that implements interface.</typeparam>
        public static IDependencyContainer RegisterTypeScoped<TType>(this IDependencyContainer container)
        {
            container.RegisterTypeScoped(typeof(TType), typeof(TType));
            return container;
        }

        /// <summary>
        /// Registers public and internal types of the given assemblies with the unity container. This is necessary
        /// to workaround the internals visible to hacks in the code base.
        /// </summary>
        /// <param name="container">Dependency container.</param>
        /// <param name="assemblies">List of assemblies in which all types should be registered with their interfaces. 
        /// This includes internal types. </param>
        /// <returns>This instance.</returns>
        public static IDependencyContainer RegisterTypesIncludingInternalsScoped(this IDependencyContainer container, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
                {
                    string interfaceName = "I" + type.Name;
                    var    interfaceType = type.GetInterface(interfaceName);
                    if (interfaceType != null)
                    {
                        container.RegisterTypeScoped(interfaceType, type);
                    }
                }
            }

            return container;
        }

        public static IDependencyContainer RegisterTypesByNameScoped(this IDependencyContainer container, Func<string, bool> checkName, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
                {
                    if (checkName(type.Name))
                    {
                        container.RegisterTypeScoped(type, type);
                    }
                }
            }

            return container;
        }

        #endregion

        /// <summary>
        /// Resolves the interface to a specific type that was registered earlier. 
        /// </summary>
        /// <param name="container">Dependency container.</param>
        /// <typeparam name="TInterface">Interface for which the registered type is looked up.</typeparam>
        /// <returns>An instance of the interface that was registered with the container earlier.</returns>
        public static TInterface Resolve<TInterface>(this IDependencyContainer container)
        {
            object obj = container.GetResolver().Resolve(typeof(TInterface));
            return (TInterface)obj;
        }

        public static TInterface Resolve<TInterface>(this IDependencyResolver resolver)
        {
            return (TInterface)resolver.Resolve(typeof(TInterface));
        }
    }
}