using Proxxon.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proxxon.Repository;
using Framework.Tools;
using System.Data.Entity;

namespace Proxxon.Logic
{
    public class MachineControler
    {
		private const string _connectionString = @"Data Source=c:\tmp\Database.sdf";
		private ProxxonRepository _repository;
        private ProxxonContext _context;

		private void Init()
		{
			AppDomain.CurrentDomain.SetData("DataDirectory", @"c:\tmp");
/*
			using (var context = new ProxxonContext(_connectionString))
			{
				System.Data.Entity.Database.SetInitializer<ProxxonContext>(new ProxxonInitializer());
				context.Database.Initialize(true);
			}        
*/
			_repository = new ProxxonRepository(_context=new ProxxonContext(_connectionString));
		}

		public DTO.Machine[] GetMachines()
		{
			Init();
			var machines = _repository.Query<Proxxon.Repository.Entities.Machine>().ToList();

			List<DTO.Machine> l = new List<DTO.Machine>();

            l.AddCloneProperties(machines);

            //foreach (Proxxon.Repository.Entities.Machine dm in machines)
            //{
            //    var x = new DTO.Machine();
            //    ObjectConverter.CopyProperties(x, dm);
            //    l.Add(x);
            //}

			return l.ToArray();
		}

        public DTO.Machine GetMachine(int id)
        {
            Init();
            var machines = _repository.Query<Proxxon.Repository.Entities.Machine>().Where((m) => m.MachineID == id).FirstOrDefault();

            return ObjectConverter.NewCloneProperties<DTO.Machine, Proxxon.Repository.Entities.Machine>(machines);
        }

        public void Update(DTO.Machine m)
        {
            Init();

            _context.Entry(m.NewCloneProperties<Proxxon.Repository.Entities.Machine, DTO.Machine>()).State = EntityState.Modified; 
            _context.SaveChanges(); 
        }
    }
}
