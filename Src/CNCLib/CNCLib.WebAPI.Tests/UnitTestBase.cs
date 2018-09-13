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

using CNCLib.WebAPI.Tests.Dependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CNCLib.WebAPI.Tests
{
    /// <summary>
    /// Base class for *all* unit tests. 
    /// </summary>
    public abstract class UnitTestBase
    {
        private static bool _globalInitialisationRun;

        [TestInitialize]
        public void InitializeTest()
        {
            if (_globalInitialisationRun)
            {
                ReInitializeCoreDependencies();
            }
            else
            {
                Framework.Tools.Dependency.Dependency.Initialize(new UnitTestDependencyProvider());
                _globalInitialisationRun = true;
            }

            InitializeCoreDependencies();
        }

        protected virtual void InitializeCoreDependencies() { }

        /// <summary>
        /// Resets the dependency container.
        /// </summary>
        private static void ReInitializeCoreDependencies()
        {
            Framework.Tools.Dependency.Dependency.Container.ResetContainer();
        }
    }
}