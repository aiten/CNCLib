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
using System.Threading.Tasks;

using Framework.Service.Abstraction;

using Microsoft.AspNetCore.Mvc;

namespace Framework.WebAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Framework.Service.Abstraction;

    using Microsoft.AspNetCore.Mvc;

    public class UriAndValue<TDto> where TDto : class
    {
        public string Uri { get; set; }

        public TDto Value { get; set; }
    }

    public static class ControllerExtension
    {
        public static string GetCurrentUri(this Controller controller)
        {
            return $"{controller.Request.Scheme}://{controller.Request.Host}{controller.Request.Path}{controller.Request.QueryString}";
        }

        public static string GetCurrentUri(this Controller controller, string removeTrailing)
        {
            if (controller.Request == null)
            {
                // unit test => no Request available 
                return "dummy";
            }

            string totalUri = controller.GetCurrentUri();
            return totalUri.Substring(0, totalUri.Length - removeTrailing.Length);
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

        #region Get/GetAll

        public static async Task<ActionResult<T>> Get<T, TKey>(this Controller controller, IGetService<T, TKey> manager, TKey id) where T : class where TKey : IComparable
        {
            var dto = await manager.Get(id);
            if (dto == null)
            {
                return controller.NotFound();
            }

            return controller.Ok(dto);
        }

        public static async Task<ActionResult<IEnumerable<T>>> GetAll<T, TKey>(this Controller controller, IGetService<T, TKey> manager) where T : class where TKey : IComparable
        {
            var dtos = await manager.GetAll();
            if (dtos == null)
            {
                return controller.NotFound();
            }

            return controller.Ok(dtos);
        }

        #endregion

        #region Add

        public static async Task<ActionResult<T>> Add<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, T value) where T : class where TKey : IComparable
        {
            TKey   newId  = await manager.Add(value);
            string newUri = controller.GetCurrentUri() + "/" + newId;
            return controller.Created(newUri, await manager.Get(newId));
        }

        public static async Task<ActionResult<IEnumerable<UriAndValue<T>>>> Add<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, IEnumerable<T> values)
            where T : class where TKey : IComparable
        {
            IEnumerable<TKey> newIds     = await manager.Add(values);
            IEnumerable<T>    newObjects = await manager.Get(newIds);

            string uri     = controller.GetCurrentUri("/bulk");
            var    newUris = newIds.Select(id => uri + "/" + id);
            var    results = newIds.Select((id, idx) => new UriAndValue<T>() { Uri = uri + "/" + id, Value = newObjects.ElementAt(idx) });
            return controller.Ok(newUris);
        }

        #endregion

        #region Update

        public static async Task<ActionResult> Update<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, TKey idFromUri, TKey idFromValue, T value)
            where T : class where TKey : IComparable
        {
            if (idFromUri.CompareTo(idFromValue) != 0)
            {
                return controller.BadRequest("Mismatch between id and dto.Id");
            }

            await manager.Update(value);
            return controller.NoContent();
        }

        public static async Task<ActionResult> Update<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, IEnumerable<T> values) where T : class where TKey : IComparable
        {
            await manager.Update(values);
            return controller.NoContent();
        }

        #endregion

        #region Delete

        public static async Task<ActionResult> Delete<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, TKey id) where T : class where TKey : IComparable
        {
            await manager.Delete(id);
            return controller.NoContent();
        }

        public static async Task<ActionResult> Delete<T, TKey>(this Controller controller, ICRUDService<T, TKey> manager, IEnumerable<TKey> ids) where T : class where TKey : IComparable
        {
            await manager.Delete(ids);
            return controller.NoContent();
        }

        #endregion
    }
}