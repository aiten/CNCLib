using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CNCLib.Logic.Contracts.DTO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CNCLib.Web.MVC.Controllers
{
    public class MachineController : Controller
    {
        private readonly string webserverurl = @"http://cnclibapi.azurewebsites.net";
		private readonly string api = @"api/machine";

		// GET: Machines
		public async Task<ActionResult> Index()
        {
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(webserverurl);
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				// New code:
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
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(webserverurl);
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				// New code:
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
/*
        // GET: Machines/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Machines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MachineID,ComPort,BaudRate,Axis,Name,SizeX,SizeY,SizeZ,SizeA,SizeB,SizeC,BufferSize,CommandToUpper,ProbeSizeX,ProbeSizeY,ProbeSizeZ,ProbeDistUp,ProbeDist,ProbeFeed,SDSupport,Spindle,Coolant,Laser,Rotate")] Machine machine)
        {
            if (ModelState.IsValid)
            {
                db.Machines.Add(machine);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(machine);
        }
*/
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
					using (var client = new HttpClient())
					{
						client.BaseAddress = new Uri(webserverurl);
						client.DefaultRequestHeaders.Accept.Clear();
						client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
/*
        // GET: Machines/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Machine machine = await db.Machines.FindAsync(id);
            if (machine == null)
            {
                return HttpNotFound();
            }
            return View(machine);
        }

        // POST: Machines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Machine machine = await db.Machines.FindAsync(id);
            db.Machines.Remove(machine);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
*/
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
