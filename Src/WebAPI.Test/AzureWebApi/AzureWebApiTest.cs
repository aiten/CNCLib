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

using System;
using System.Net.Http;
using System.Net.Http.Headers;

using Framework.Tools;

namespace CNCLib.WebAPI.Test.AzureWebApi
{
    public class AzureWebApiTest : UnitTestBase
    {
        //public static readonly string AzureUri = @"http://localhost:55586/";
        public static readonly string _AzureUri = @"https://cnclib.azurewebsites.net/";

        public HttpClient _httpClient;

        protected HttpClient GetHttpClient()
        {
            if (_httpClient == null)
            {
                _httpClient             = new HttpClient();
                _httpClient.BaseAddress = new Uri(_AzureUri);
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Base64Helper.StringToBase64($"{"CNCLibGlobal"}:{"CNCLib4Global."}"));
            }


            return _httpClient;
        }
    }
}