using Proxxon.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proxxon.Repository;
using Framework.Tools;

namespace Proxxon.Logic
{
    public class MachineControler
    {
		private const string _connectionString = @"Data Source=c:\tmp\Database.sdf";
		private ProxxonRepository _repository;

		private void Init()
		{
			AppDomain.CurrentDomain.SetData("DataDirectory", @"c:\tmp");

			using (var context = new ProxxonContext(_connectionString))
			{
				System.Data.Entity.Database.SetInitializer<ProxxonContext>(new ProxxonInitializer());
				context.Database.Initialize(true);
			}        

			_repository = new ProxxonRepository(new ProxxonContext(_connectionString));
		}

		public DTO.Machine[] GetMachines()
		{
			Init();
			var machines = _repository.Query<Proxxon.Repository.Entities.Machine>().ToList();

			List<DTO.Machine> l = new List<DTO.Machine>();

			foreach (Proxxon.Repository.Entities.Machine dm in machines)
			{
				var x = new DTO.Machine();
				ObjectMapper.MyCopyProperties(x, dm);
				l.Add(x);
			}

			return l.ToArray();
		}
    }
}
