use WareHouseDB
go

insert into Contexts (ContextCode, ContextName, IsEnabled) values
	(N'INBOUND', N'Ingresso merce', 1),
	(N'OUTBOUND', N'Uscita merce', 1),
	(N'INVENTORY', N'Inventario', 1),
    (N'ERRORSIM',  N'Simulazione errore', 1);
go