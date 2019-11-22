# Please set in Startup.cs: services.AddDbContext<CNCLibContext>(options => SqLiteDatabaseTools.OptionBuilder(options));
Add-Migration -Name V3 -StartupProject Repository\Repository.Migrations.Startup -Project Repository\Repository.SqLite -OutputDir Migrations
pause