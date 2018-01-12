using System;
using CNCLib.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;


namespace CNCLib.Repository.Migrate
{
    class Program
    {
        public class MyCNCLibContext : CNCLibContext
        {
            public MyCNCLibContext()
            {
                CNCLibContext.OnConfigure = (optionsBuilder) =>
                {
                    //                    optionsBuilder.UseSqlServer(@"Data Source = (LocalDB)\MSSQLLocalDB; Initial Catalog = CNCLibTestNew; Integrated Security = True");
                    //optionsBuilder.UseSqlCe($@"Data Source={System.IO.Path.GetTempPath()}\CNCLibTest.sdf");
                    optionsBuilder.UseSqlite($"Data Source={System.IO.Path.GetTempPath()}\\CNCLib.db");
                };
            }
        }

        static void Main(string[] args)
        {
            using (var db = new MyCNCLibContext())
            {
                db.Database.Migrate();
                CNCLibDefaultData.CNCSeed(db);
                db.SaveChanges();

                Console.WriteLine();
                Console.WriteLine("All machines in database:");
                foreach (var m in db.Machines)
                {
                    Console.WriteLine(" - {0}", m.Name);
                }
            }
        }
    }
}
