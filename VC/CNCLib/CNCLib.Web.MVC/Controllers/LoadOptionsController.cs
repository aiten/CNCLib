////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using CNCLib.Logic.Contracts.DTO;

namespace CNCLib.Web.MVC.Controllers
{
    public class LoadOptionsController : Controller
    {
		private readonly string _webserverurl = @"http://cnclibapi.azurewebsites.net";
		private readonly string _api = @"api/LoadOptions";

		private System.Net.Http.HttpClient CreateHttpClient()
		{
			var client = new HttpClient();
			client.BaseAddress = new Uri(_webserverurl);
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			return client;
		}

		private async Task<LoadOptions> Get(int id)
		{
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(_api + "/" + id);
				if (response.IsSuccessStatusCode)
				{
					LoadOptions value = await response.Content.ReadAsAsync<LoadOptions>();

					return value;
				}
			}
			return null;
		}

		// GET: LoadOptions
		public async Task<ActionResult> Index()
		{
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(_api);
				if (response.IsSuccessStatusCode)
				{
					IEnumerable<LoadOptions> infos = await response.Content.ReadAsAsync<IEnumerable<LoadOptions>>();
					return View(infos);
				}
			}

			return View();
		}

		// GET: LoadOptions/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			LoadOptions loadInfo = await Get(id.Value);
			if (loadInfo == null)
			{
				return HttpNotFound();
			}
			return View(loadInfo);
		}

		// GET: LoadOptions/Create
		public ActionResult Create()
		{
			return View(new LoadOptions());
		}


		// POST: LoadOptions/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([Bind(Include = "Id,LoadType,FileName,FileContent,SettingName,GCodeWriteToFileName,SwapXY,ScaleX,ScaleY,OfsX,OfsY,AutoScale,AutoScaleKeepRatio,AutoScaleSizeX,AutoScaleSizeY,AutoScaleBorderDistX,AutoScaleBorderDistY,PenMoveType,EngravePosInParameter,EngravePosUp,EngravePosDown,MoveSpeed,EngraveDownSpeed,LaserFirstOnCommand,LaserOnCommand,LaserOffCommand,LaserSize,ImageWriteToFileName,GrayThreshold,ImageDPIX,ImageDPIY,ImageInvert,Dither,NewspaperDitherSize,DotDistX,DotDistY,DotSizeX,DotSizeY,UseYShift,RotateHeart,HoleType")] LoadOptions loadInfo)
		{
			if (ModelState.IsValid)
			{
				using (var client = CreateHttpClient())
				{
					HttpResponseMessage response = await client.PostAsJsonAsync(_api, loadInfo);

					if (response.IsSuccessStatusCode)
						return RedirectToAction("Index");
				}
			}

			return View(loadInfo);
		}

		// GET: LoadOptions/Edit/5
		public async Task<ActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			LoadOptions loadInfo = await Get(id.Value);
			if (loadInfo == null)
			{
				return HttpNotFound();
			}
			return View(loadInfo);
		}

		// POST: LoadOptions/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit([Bind(Include = "Id,LoadType,FileName,FileContent,SettingName,GCodeWriteToFileName,SwapXY,ScaleX,ScaleY,OfsX,OfsY,AutoScale,AutoScaleKeepRatio,AutoScaleSizeX,AutoScaleSizeY,AutoScaleBorderDistX,AutoScaleBorderDistY,PenMoveType,EngravePosInParameter,EngravePosUp,EngravePosDown,MoveSpeed,EngraveDownSpeed,LaserFirstOnCommand,LaserOnCommand,LaserOffCommand,LaserSize,ImageWriteToFileName,GrayThreshold,ImageDPIX,ImageDPIY,ImageInvert,Dither,NewspaperDitherSize,DotDistX,DotDistY,DotSizeX,DotSizeY,UseYShift,RotateHeart,HoleType")] LoadOptions loadInfo)
		{
			if (ModelState.IsValid)
			{
				using (var client = CreateHttpClient())
				{
					var response = await client.PutAsJsonAsync(_api + "/" + loadInfo.Id, loadInfo);

					if (response.IsSuccessStatusCode)
					{
						return RedirectToAction("Index");
					}
				}
			}
			return View(loadInfo);
		}

		// GET: LoadOptions/Delete/5
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.DeleteAsync(_api + "/" + id);

				if (response.IsSuccessStatusCode)
				{
					return RedirectToAction("Index");
				}
				return HttpNotFound();
			}
		}

		// POST: LoadOptions/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.DeleteAsync(_api + "/" + id);

				if (response.IsSuccessStatusCode)
				{
					return RedirectToAction("Index");
				}
				return HttpNotFound();
			}
		}
	}
}
