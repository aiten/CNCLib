-nouseregional

#exporttablecsv %tmp%\Configuration.csv=select * from configuration;
#exporttablecsv %tmp%\Item.csv=select * from Item;
#exporttablecsv %tmp%\ItemProperty.csv=select * from ItemProperty;
#exporttablecsv %tmp%\Log.csv=select * from Log;

#exporttablecsv %tmp%\Machine.csv=select * from Machine;
#exporttablecsv %tmp%\MachineCommand.csv=select * from MachineCommand;
#exporttablecsv %tmp%\MachineInitCommand.csv=select * from MachineInitCommand;

#exporttablecsv %tmp%\User.csv=select * from [User];
