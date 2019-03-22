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

    using Abstraction;

    /// <summary>
    /// Static class that gives access to the DependencyContainer of the module. 
    /// Further offers methods to simplify dealing with often needed tasks.
    /// </summary>
    public static class Dependency
    {
        private static IDependencyProvider _provider;

        /// <summary>
        /// Test if container is initialized.
        /// </summary>
        public static bool IsInitialized => _provider != null;

        /// <summary>
        /// Returns the DependencyContainer that should be used to register and resolve dependencies.
        /// </summary>
        public static IDependencyContainer Container => _provider.Container;

        /// <summary>
        /// Initializes the class.
        /// </summary>
        /// <param name="provider">Class that provides the dependency container.</param>
        public static void Initialize(IDependencyProvider provider)
        {
            if (_provider != null)
            {
                throw new InvalidOperationException("Cannot initialize Dependency class twice.");
            }

            _provider = provider;
        }

        /// <summary>
        /// Resolves a dependency by forwarding the call to the <see cref="Container"/>.
        /// </summary>
        /// <typeparam name="TInterface">Interface to be resolved.</typeparam>
        public static TInterface Resolve<TInterface>()
        {
            return Container.Resolve<TInterface>();
        }
    }
}