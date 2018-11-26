////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

namespace Framework.Dependency
{
    using System;

    using Framework.Dependency.Abstraction;

    /// <summary>
    /// Static class that gives access to the DependencyContainer of the module. 
    /// Further offers methods to simplify dealing with often needed tasks.
    /// </summary>
    public static class Dependency
    {
        private static IDependencyProvider _provider;

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
        /// Test if container is initialized.
        /// </summary>
        public static bool IsInitialized => _provider != null;

        /// <summary>
        /// Returns the DependencyContainer that should be used to register and resolve dependencies.
        /// </summary>
        public static IDependencyContainer Container => _provider.Container;

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