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

using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using CNCLib.Service.Abstraction;

using Framework.Pattern;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

namespace CNCLib.WpfClient.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IFactory<IUserService> _userService;

        #region crt

        public LoginViewModel(IFactory<IUserService> userService)
        {
            _userService = userService;
        }

        #endregion

        #region Properties

        private string _userName;

        public string UserName
        {
            get => _userName;
            set { SetProperty(() => _userName == value, () => _userName = value); }
        }

        private string _password;

        public string Password
        {
            get => _password;
            set { SetProperty(() => _password == value, () => _password = value); }
        }

        #endregion

        #region Operations

        public async Task<bool> VerifyUser(CancellationToken ctk)
        {
            using (var scope = _userService.Create())
            {
                var isValidUser = await scope.Instance.IsValidUser(UserName, Password);

                if (isValidUser)
                {
                    DialogOKAction();
                    return true;
                }

                MessageBox?.Invoke(@"Illegal user/password", @"Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool CanVerifyUser()
        {
            return true;
        }

        #endregion

        #region Commands

        public ICommand VerifyLoginCommand => new DelegateCommandAsync<bool>(VerifyUser, CanVerifyUser);

        #endregion
    }
}