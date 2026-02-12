USE BankDB
GO

DELETE FROM Transactions;
DELETE FROM Accounts;
DELETE FROM Branches;
DELETE FROM Clients;

DBCC CHECKIDENT ('Clients', RESEED, 0);
DBCC CHECKIDENT ('Branches', RESEED, 0);
DBCC CHECKIDENT ('Accounts', RESEED, 0);
DBCC CHECKIDENT ('Transactions', RESEED, 0);
GO

INSERT INTO Clients (ClientName, ClientSurname, ClientTaxCode) VALUES 
('Mario', 'Rossi', 'RSSMRA80A01H501U'),
('Laura', 'Bianchi', 'BNCLRA85B41L219Z'),
('Giuseppe', 'Verdi', 'VRDGPP70C15F205H'),
('Elena', 'Neri', 'NRELNE90D20H501X'),
('Marco', 'Gialli', 'GLLMRC75E10L219W');

INSERT INTO Branches (BranchName, BranchAddress) VALUES 
('Sede Centrale', 'Via Roma 1'),
('Filiale Nord', 'Via Milano 120'),
('Filiale Sud', 'Corso Italia 50'),
('Ufficio Digital', 'Piazza Duomo 5');

INSERT INTO Accounts (ClientID, BranchID, Balance, IBAN) VALUES 
(1, 1, 5200.50, 'IT12A0123401234000000010001'),
(1, 2, 150.00,  'IT12B0123401234000000010002'),
(2, 1, 12800.00,'IT12C0123401234000000010003'),
(3, 3, 450.75,  'IT12D0123401234000000010004'),
(4, 4, 3200.00, 'IT12E0123401234000000010005'),
(5, 3, 980.10,  'IT12F0123401234000000010006');

INSERT INTO Transactions (AccountID, TransactionAmount, TransactionType, TransactionDate) VALUES 
(1, 1500.00, 'Entrata', '2026-01-10 10:30:00'),
(1, -200.00, 'Uscita', '2026-01-12 15:45:00'),
(2, 50.00, 'Entrata', '2026-01-15 09:00:00'),
(3, 3000.00, 'Entrata', '2026-02-01 11:20:00'),
(3, -150.50, 'Uscita', '2026-02-05 18:10:00'),
(4, 100.00, 'Entrata', '2026-02-10 14:00:00'),
(5, 2500.00, 'Entrata', GETDATE()),
(5, -300.00, 'Uscita', GETDATE());
GO