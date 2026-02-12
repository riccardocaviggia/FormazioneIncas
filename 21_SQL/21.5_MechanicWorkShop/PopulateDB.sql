USE MechanicalWorkShop
GO

DELETE FROM WorkDetails;
DELETE FROM Replacements;
DELETE FROM Works;
DELETE FROM Mechanics;
DELETE FROM Vehicles;
DELETE FROM Clients;

DBCC CHECKIDENT ('Clients', RESEED, 0);
DBCC CHECKIDENT ('Vehicles', RESEED, 0);
DBCC CHECKIDENT ('Mechanics', RESEED, 0);
DBCC CHECKIDENT ('Works', RESEED, 0);
DBCC CHECKIDENT ('Replacements', RESEED, 0);
GO

INSERT INTO Clients (ClientName, ClientSurname, ClientTelephone) VALUES 
('Mario', 'Rossi', '3331122334'),
('Giulia', 'Bianchi', '3475566778'),
('Luca', 'Verdi', '3209988776'),
('Elena', 'Neri', '3384455661'),
('Marco', 'Gialli', '3291133557'),
('Sofia', 'Russo', '3402244668'),
('Alessandro', 'Gallo', '3317788990'),
('Chiara', 'Esposito', '3456677882');

INSERT INTO Vehicles (ClientID, VehiclePlate, VehicleBrand) VALUES 
(1, 'AA123BB', 'Fiat'), (1, 'BX987ZZ', 'Alfa Romeo'),
(2, 'CC456DD', 'Dacia'), 
(3, 'EE789FF', 'Mini'), (3, 'ST555UV', 'Fiat'),
(4, 'GG012HH', 'VolksWagen'), 
(5, 'II345LL', 'BMW'), 
(6, 'MM678NN', 'VolksWagen'), 
(7, 'OO901PP', 'Fiat'), 
(8, 'QQ234RR', 'Mini');


INSERT INTO Mechanics (MechanicName, MechanicSurname) VALUES 
('Roberto', 'Cacciavite'),
('Francesco', 'Bullone'),
('Andrea', 'Saldatura'),
('Valeria', 'Chiave');

INSERT INTO Replacements (ReplacementCode, ReplacementName, ReplacementPrice) VALUES 
('OIL-5W30', 'Olio Motore 5W30', 16.00),
('FIL-AIR', 'Filtro Aria', 25.50),
('FIL-OIL', 'Filtro Olio', 14.00),
('BRK-PAD-F', 'Pastiglie Freni Ant.', 55.00),
('BRK-PAD-R', 'Pastiglie Freni Post.', 45.00),
('WIP-BLD', 'Spazzole Tergi', 19.50),
('BATT-80A', 'Batteria 80Ah', 120.00),
('TYRE-S', 'Pneumatico Estivo', 85.00),
('LUMP-H7', 'Lampadina H7', 9.00),
('ANT-FRZ', 'Liquido Radiatore', 12.50),
('FR-DISK', 'Disco Freno', 75.00),
('SP-PLUG', 'Candela Accensione', 11.00);

INSERT INTO Works (VehicleID, MechanicID, WorkDate, WorkCost, Description) VALUES 
(1, 1, '2026-01-10 09:00:00', 90.00, 'Tagliando e Filtri'),       
(3, 2, '2026-01-12 14:00:00', 120.00, 'Revisione Freni'),    
(4, 3, '2026-01-15 10:30:00', 40.00, 'Sostituzione Batteria'),   
(5, 4, '2026-01-20 08:45:00', 200.00, 'Cambio Gomme'),   
(2, 1, '2026-01-22 16:00:00', 30.00, 'Cambio Lampadina'),          
(6, 2, '2026-01-25 11:15:00', 85.00, 'Tagliando Standard'),    
(7, 3, '2026-02-01 09:30:00', 150.00, 'Sostituzione Dischi'),    
(8, 4, '2026-02-03 15:00:00', 60.00, 'Rabbocco Liquidi'),      
(10, 1, '2026-02-05 10:00:00', 110.00, 'Manutenzione Accensione'), 
(9, 2, '2026-02-08 14:20:00', 55.00, 'Cambio Tergicristalli'),   
(1, 3, '2026-02-10 12:00:00', 45.00, 'Sostituzione Filtro Aria'),  
(3, 4, GETDATE(), 70.00, 'Controllo Generale');                  

INSERT INTO WorkDetails (WorkID, ReplacementID, Quantity) VALUES 
(1, 1, 5), (1, 3, 1), (1, 2, 1), 
(2, 4, 2), (2, 5, 2),           
(3, 7, 1),                    
(4, 8, 4),
(5, 9, 2),                      
(6, 1, 4), (6, 3, 1),           
(7, 11, 2), (7, 4, 2),      
(8, 10, 2), (8, 9, 1),       
(9, 12, 4), (9, 1, 1),     
(10, 6, 2),              
(11, 2, 1),                
(12, 10, 1), (12, 9, 1);    
GO