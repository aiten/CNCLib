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
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Framework.Contracts.Logic;
using Framework.Contracts.Service;
using Microsoft.AspNetCore.Mvc;

namespace Framework.Web
{
    public static class ControllerExtension
    {
        public static string GetCurrentUri(this Controller controller)
        {
            return $"{controller.Request.Scheme}://{controller.Request.Host}{controller.Request.Path}{controller.Request.QueryString}";
        }

        public static string GetCurrentUri(this Controller controller, string removetraining)
        {
            if (controller.Request == null)
            {
                // unit test => no Request available 
                return "dummy";
            }

            string totaluri = controller.GetCurrentUri();
            return totaluri.Substring(0, totaluri.Length - removetraining.Length);
        }

        public static async Task<ActionResult<T>> NotFoundOrOk<T>(this Controller controller, T obj)
        {
            if (obj == null)
            {
                await Task.Delay(0); // avoid CS1998
                return controller.NotFound();
            }

            return controller.Ok(obj);
        }

        public static ActionResult GenerateErrorResponse(this Controller controller, Exception ex)
        {
            //		    logger.Log(ex);
            //return controller.InternalServerError(ex);
            //return controller.Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            return null;
        }

        #region Get/GetAll

        public static async Task<ActionResult> Get<T,TKey>(this Controller controller, IGetService<T, TKey> manager, TKey id)
            where T : class
            where TKey : IComparable
        {
            try
            {
                var dto = await manager.Get(id);
                if (dto == null)
                {
                    return controller.NotFound();
                }

                return controller.Ok(dto);
            }
            catch (Exception ex)
            {
                return controller.GenerateErrorResponse(ex);
            }
        }

        #endregion

        #region Add

        public static async Task<ActionResult> Add<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, T value)
            where T : class
            where TKey : IComparable
        {
            if (!controller.ModelState.IsValid || value == null)
            {
                return controller.BadRequest(controller.ModelState);
            }
            try
            {
                TKey newid = await manager.Add(value);
                string newuri = controller.GetCurrentUri() + "/" + newid;
                return controller.Created(newuri, await manager.Get(newid));
            }
            catch (Exception ex)
            {
                return controller.GenerateErrorResponse(ex);
            }
        }
/*
        public static async Task<ActionResult> Add<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, IEnumerable<T> values)
            where T : class
            where TKey : IComparable
        {
            if (!controller.ModelState.IsValid || values == null)
            {
                return controller.BadRequest(controller.ModelState);
            }
            try
            {
                string uri = controller.GetCurrentUri("/bulk");
                IEnumerable<TKey> newids = await manager.Add(values);
                var newuris = newids.Select(id => uri + "/" + id);
                return controller.Ok(newuris);
            }
            catch (Exception ex)
            {
                return controller.GenerateErrorResponse(ex);
            }
        }
*/
        #endregion

        #region Update
/*
        public static async Task<ActionResult> Update<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, IEnumerable<T> values)
            where T : class
            where TKey : IComparable
        {
            if (!controller.ModelState.IsValid || values == null)
            {
                return controller.BadRequest(controller.ModelState);
            }
            try
            {
                await manager.Update(values);
                return controller.StatusCode(System.Net.HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return controller.GenerateErrorResponse(ex);
            }
        }
*/
        public static async Task<ActionResult> Update<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, TKey idFromUri, TKey idFromValue, T value)
            where T : class
            where TKey : IComparable
        {
            if (!controller.ModelState.IsValid || value == null)
            {
                return controller.BadRequest(controller.ModelState);
            }

            if (idFromUri.CompareTo(idFromValue) != 0)
            {
                return controller.BadRequest("Missmatch between id and dto.Id");
            }

            try
            {
                await manager.Update(value);
                return controller.NoContent();
            }
            catch (Exception ex)
            {
                return controller.GenerateErrorResponse(ex);
            }
        }

        #endregion

        #region Delete

        public static async Task<ActionResult> Delete<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, TKey id)
            where T : class
            where TKey : IComparable
        {
            try
            {
                await manager.Delete(id);
                return controller.NoContent();
            }
            catch (Exception ex)
            {
                return controller.GenerateErrorResponse(ex);
            }
        }

        /*
        public static async Task<ActionResult> Delete<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, IEnumerable<TKey> ids)
            where T : class
            where TKey : IComparable
        {
            try
            {
                await manager.Delete(ids);
                return controller.StatusCode(System.Net.HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return controller.GenerateErrorResponse(ex);
            }
        }
        */
        #endregion



        public static async Task<ActionResult<IEnumerable<T>>> GetAll<T>(this Controller controller, IRest<T> rest) 
		{
		    IEnumerable<T> all = await rest.Get();
		    return await NotFoundOrOk(controller, all);
		}

        public static async Task<ActionResult<T>> GetREST<T>(this Controller controller, IRest<T> rest, int id)
        {
            T obj = await rest.Get(id);
            return await NotFoundOrOk(controller,obj);
        }

        public static async Task<ActionResult<T>> PostREST<T>(this Controller controller, IRest<T> rest, T obj)
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

        public static async Task<ActionResult<T>> PutRest<T>(this Controller controller, IRest<T> rest, int id, T value)
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

        public static async Task<ActionResult<T>> DeleteREST<T>(this Controller controller, IRest<T> rest, int id)
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