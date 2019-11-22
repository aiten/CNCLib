# Please set in Startup.cs: services.AddDbContext<CNCLibContext>(options => SqlServerDatabaseTools.OptionBuilder(options));
Add-Migration -Name V3 -StartupProject Repository\Repository.Migrations.Startup -Project Repository\Repository.SqlServer -OutputDir Migrations
pause