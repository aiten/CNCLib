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
    public class ControlerBase
    {
		private const string _connectionString = @"Data Source=c:\tmp\Database.sdf";
		private ProxxonRepository _repository;
        private ProxxonContext _context;

        public ProxxonRepository Repository 
        { 
            get
            {
                if (_repository==null) Init();
                return _repository;
            }
        }
        public ProxxonContext Context
        {
            get
            {
                if (_context==null) Init();
                return _context;
            }
         }

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
    }
}
