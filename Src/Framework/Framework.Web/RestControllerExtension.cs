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

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Framework.Web
{
    public static class RestControllerExtension
    {
        public static string GetCurrentUri(this Controller controller)
        {
            return $"{controller.Request.Scheme}://{controller.Request.Host}{controller.Request.Path}{controller.Request.QueryString}";
        }

        public static async Task<IActionResult> NotFoundOrOk<T>(this Controller controller, T obj)
        {
            if (obj == null)
            {
                await Task.Delay(0); // avoid CS1998
                return controller.NotFound();
            }

            return controller.Ok(obj);
        }

        public static async Task<IActionResult> GetAll<T>(this Controller controller, IRest<T> rest) 
		{
		    IEnumerable<T> all = await rest.Get();
		    return await NotFoundOrOk<IEnumerable<T>>(controller, all);
		}

        public static async Task<IActionResult> Get<T>(this Controller controller, IRest<T> rest, int id)
        {
            T obj = await rest.Get(id);
            return await NotFoundOrOk<T>(controller,obj);
        }

        public static async Task<IActionResult> Post<T>(this Controller controller, IRest<T> rest, T obj)
        {
            if (!controller.ModelState.IsValid || obj == null)
            {
                return controller.BadRequest(controller.ModelState);
            }
            try
            {
                int newid = await rest.Add(obj);
                return controller.Created($@"{controller.GetCurrentUri()}/{newid}", await rest.Get(newid));
            }
            catch (Exception ex)
            {
                return controller.BadRequest(ex.Message);
            }
        }

        public static async Task<IActionResult> Put<T>(this Controller controller, IRest<T> rest, int id, T value)
        {
            if (!controller.ModelState.IsValid || value == null)
            {
                return controller.BadRequest(controller.ModelState);
            }

            try
            {
                if (rest.CompareId(id, value) == false)
                {
                    return controller.BadRequest("Missmatch between id and machineID");
                }

                await rest.Update(id, value);
                return controller.StatusCode(204);
            }
            catch (Exception ex)
            {
                return controller.BadRequest(ex.Message);
            }
        }

        public static async Task<IActionResult> Delete<T>(this Controller controller, IRest<T> rest, int id)
        {
            T value = await rest.Get(id);
            if (value == null)
            {
                return controller.NotFound();
            }

            await rest.Delete(id, value);
            return controller.Ok(value);
        }
    }
}