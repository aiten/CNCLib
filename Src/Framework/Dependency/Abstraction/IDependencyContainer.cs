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

namespace Framework.Dependency.Abstraction
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Inversion of Control container, which enables Dependency Injection. 
    /// </summary>
    public interface IDependencyContainer : IDisposable
    {
        /// <summary>
        /// Gets an enumeration containing all types registered with the dependency container.
        /// </summary>
        IEnumerable<Type> RegisteredTypes { get; }

        /// <summary>
        /// Registers the given object for the interface. 
        /// </summary>
        /// <param name="typeFrom">Type from.</param>
        /// <param name="obj">Object that should be returned for Resolve&lt;TInterface&gt;() calls.</param>
        /// <returns>This instance.</returns>
        IDependencyContainer RegisterInstance(Type typeFrom, object obj);

        /// <summary>
        /// Register typeFrom to typeTo.
        /// </summary>
        /// <param name="typeFrom">Type from.</param>
        /// <param name="typeTo">Type to.</param>
        /// <returns></returns>
        IDependencyContainer RegisterType(Type typeFrom, Type typeTo);

        /// <summary>
        /// Register typeFrom to typeTo.
        /// </summary>
        /// <param name="typeFrom">Type from.</param>
        /// <param name="typeTo">Type to.</param>
        /// <returns></returns>
        IDependencyContainer RegisterTypeScoped(Type typeFrom, Type typeTo);

        /// <summary>
        /// This can be called in unit tests to reset the container to an empty state. 
        /// NOTE: After calling this you should call the module's DependencyInitializer again.
        /// </summary>
        void ResetContainer();

        /// <summary>
        /// Get an instance of an object which can resolve the "service" (object). 
        /// </summary>
        IDependencyResolver GetResolver();
    }
}