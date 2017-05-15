select @@version;

select * from __MigrationHistory;

--#exit;


drop table Configuration;
drop table __MigrationHistory;
drop table ItemProperty;
drop table Item;
drop table MachineCommand;
drop table MachineInitCommand;
drop table Machine;
drop table [User];
