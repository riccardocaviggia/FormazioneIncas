USE LibraryDB;
GO

DELETE FROM BookLoans;
DELETE FROM BookAuthors;
DELETE FROM Books;
DELETE FROM Authors;
DELETE FROM Users;

DBCC CHECKIDENT ('Authors', RESEED, 0);
DBCC CHECKIDENT ('Books', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('BookLoans', RESEED, 0);
GO

INSERT INTO Authors (AuthorName, AuthorSurname) VALUES 
('Umberto', 'Eco'),
('Italo', 'Calvino'),
('Natalia', 'Ginzburg'),
('Primo', 'Levi'),
('Giacomo', 'Leopardi'),
('Grazia', 'Deledda'),
('Dante', 'Alighieri'),
('Giovanni', 'Boccaccio'),
('Alessandro', 'Manzoni'),
('Carlo', 'Fruttero'),
('Franco', 'Lucentini');

INSERT INTO Books (BookTitle, BookISBN) VALUES 
('Il nome della rosa', '9788845278655'),
('Il pendolo di Foucault', '9788845278656'),
('Baudolino', '9788845278657'),
('Il cimitero di Praga', '9788845278658'),
('Il barone rampante', '9788804668231'),
('Le città invisibili', '9788804668232'),
('Lessico famigliare', '9788804668233'),
('Se questo è un uomo', '9788804668234'),
('La tregua', '9788804668235'),
('Canti', '9788804668236'),
('Canne al vento', '9788804668237'),
('Divina Commedia', '9788804668238'),
('Decameron', '9788804668239'),
('I Promessi Sposi', '9788804668240'),
('La donna della domenica', '9788804668241');

INSERT INTO BookAuthors (BookID, AuthorID) VALUES 
(1, 1), (2, 1), (3, 1), (4, 1),
(1, 2),
(5, 2), (6, 2),
(7, 3),
(8, 4), (9, 4),
(10, 5),
(11, 6),
(12, 7),
(13, 8),
(14, 9),
(15, 10),
(15, 11);

INSERT INTO Users (UserName, UserSurname, UserEmail) VALUES 
('Mario', 'Rossi', 'mario.rossi@email.it'),
('Giulia', 'Verdi', 'giulia.verdi@email.it'),
('Luca', 'Neri', 'luca.neri@email.it'),
('Elena', 'Bianchi', 'elena.bianchi@email.it'),
('Marco', 'Gialli', 'marco.gialli@email.it');

INSERT INTO BookLoans (BookID, UserID, LoanDate, ReturnDate) VALUES 
(1, 1, '2026-01-05', '2026-01-20'),
(2, 1, '2026-01-21', '2026-02-05'),
(3, 1, '2026-02-06', '2026-02-28'),
(4, 1, '2026-03-01', NULL),
(5, 2, '2026-01-10', '2026-01-25'),
(6, 2, '2026-02-01', NULL),
(12, 3, '2025-12-15', '2026-01-10'),
(14, 3, '2026-01-15', NULL),
(8, 4, '2026-02-10', '2026-03-01'),
(9, 4, '2026-03-05', NULL),
(11, 5, '2026-01-20', '2026-02-15'),
(7, 5, '2026-02-20', NULL),
(13, 1, '2026-03-10', NULL);