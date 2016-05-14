////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
	public class MachineController : Controller
    {
        private readonly string webserverurl = @"http://cnclibapi.azurewebsites.net";
		private readonly string api = @"api/machine";

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(webserverurl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
       }

        // GET: Machines
        public async Task<ActionResult> Index()
        {
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(api);
				if (response.IsSuccessStatusCode)
				{
					IEnumerable<Machine> machines = await response.Content.ReadAsAsync<IEnumerable<Machine>>();
					return View(machines);
				}
			}

			return View();
        }

		private async Task<Machine> GetMachine(int id)
		{
            using (var client = CreateHttpClient())
            {
				HttpResponseMessage response = await client.GetAsync(api + "/" + id);
				if (response.IsSuccessStatusCode)
				{
					Machine machine = await response.Content.ReadAsAsync<Machine>();

					return machine;
				}
			}
			return null;
		}

		// GET: Machines/Details/5
		public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

			Machine machine = await GetMachine(id.Value);
			if (machine != null)
			{
				return View(machine);
			}
			return HttpNotFound();
        }

        // GET: Machines/Create
        public ActionResult Create()
        {
            var machine = new Machine()
            {
                Name = "New",
                ComPort = "comX",
                Axis = 3,
                SizeX = 130m,
                SizeY = 45m,
                SizeZ = 81m,
                SizeA = 360m,
                SizeB = 360m,
                SizeC = 360m,
                BaudRate = 115200,
                BufferSize = 63,
                CommandToUpper = false,
                ProbeSizeZ = 25,
                ProbeDist = 10m,
                ProbeDistUp = 3m,
                ProbeFeed = 100m,
                SDSupport = true,
                Spindle = true,
                Coolant = true,
                Rotate = true,
                Laser = false,
                MachineCommands = new MachineCommand[0],
                MachineInitCommands = new MachineInitCommand[0]
            };
            return View(machine);
        }

        // POST: Machines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Create([Bind(Include = "MachineID,ComPort,BaudRate,Axis,Name,SizeX,SizeY,SizeZ,SizeA,SizeB,SizeC,BufferSize,CommandToUpper,ProbeSizeX,ProbeSizeY,ProbeSizeZ,ProbeDistUp,ProbeDist,ProbeFeed,SDSupport,Spindle,Coolant,Laser,Rotate")] Machine machine)
        public async Task<ActionResult> Create(Machine machine)
        {
            if (ModelState.IsValid)
            {
                if (machine.MachineCommands == null) machine.MachineCommands = new MachineCommand[0];
                if (machine.MachineInitCommands == null) machine.MachineInitCommands = new MachineInitCommand[0];

                using (var client = CreateHttpClient())
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(api, machine);

                    if (response.IsSuccessStatusCode)
                        return RedirectToAction("Index");
                }
            }

            return View(machine);
        }

        // GET: Machines/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

			Machine machine = await GetMachine(id.Value);
			if (machine != null)
			{
				return View(machine);
			}
			return HttpNotFound();
        }

        // POST: Machines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Machine machine)
//		public async Task<ActionResult> Edit([Bind(Include = "MachineID,ComPort,BaudRate,Axis,Name,SizeX,SizeY,SizeZ,SizeA,SizeB,SizeC,BufferSize,CommandToUpper,ProbeSizeX,ProbeSizeY,ProbeSizeZ,ProbeDistUp,ProbeDist,ProbeFeed,SDSupport,Spindle,Coolant,Laser,Rotate")] Machine machine)
		{
			if (ModelState.IsValid)
            {
				Machine machineindDB = await GetMachine(machine.MachineID);
				if (machineindDB != null)
				{
					if (machine.MachineCommands == null)
					{
						machine.MachineCommands = machineindDB.MachineCommands;
					}
					if (machine.MachineInitCommands == null)
					{
						machine.MachineInitCommands = machineindDB.MachineInitCommands;
					}
                    using (var client = CreateHttpClient())
                    {
						var response = await client.PutAsJsonAsync(api + "/" + machine.MachineID, machine);

						if (response.IsSuccessStatusCode)
						{
							return RedirectToAction("Index");
						}
					}
				}
            }
            return View(machine);
        }

        // GET: Machines/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(api + "/" + id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }
        }

        // POST: Machines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(api + "/" + id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
//                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
