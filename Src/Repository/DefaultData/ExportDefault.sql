-nouseregional
-UniCode

#exporttablecsv %thisfiledir%\Configuration.csv=select * from configuration;
#exporttablecsv %thisfiledir%\Item.csv=select * from Item;
#exporttablecsv %thisfiledir%\ItemProperty.csv=select * from ItemProperty;
#exporttablecsv %thisfiledir%\Log.csv=select * from Log where 1=0;

#exporttablecsv %thisfiledir%\Machine.csv=select * from Machine;
#exporttablecsv %thisfiledir%\MachineCommand.csv=select * from MachineCommand;
#exporttablecsv %thisfiledir%\MachineInitCommand.csv=select * from MachineInitCommand;

#exporttablecsv %thisfiledir%\User.csv=select * from [User];
--#exporttablecsv %thisfiledir%\UserFile.csv=select * from [UserFile];
