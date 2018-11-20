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
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Dependency container for use in Live. Throws an exception when a Type cannot be resolved.
    /// </summary>
    public class MsDependencyScope : IDependencyScope
    {
        private readonly IServiceScope _serviceScope;

        public  IDependencyResolver Resolver { get; private set; }

        public MsDependencyScope(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
            Resolver = new MsDependencyResolver(_serviceScope.ServiceProvider);
        }

        #region IDisposable Support

        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _serviceScope.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}