USE StreamingPlatform
GO

DELETE FROM Reviews;
DELETE FROM Casts;
DELETE FROM Episodes;
DELETE FROM Users;
DELETE FROM Contents;
DELETE FROM Actors;

DBCC CHECKIDENT ('Actors', RESEED, 0);
DBCC CHECKIDENT ('Contents', RESEED, 0);
DBCC CHECKIDENT ('Episodes', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Reviews', RESEED, 0);
GO

INSERT INTO Actors (ActorName, ActorSurname) VALUES 
('Cillian', 'Murphy'), ('Robert', 'Downey Jr.'), ('Florence', 'Pugh'), -- Oppenheimer
('Bryan', 'Cranston'), ('Aaron', 'Paul'),                           -- Breaking Bad
('Pedro', 'Pascal'), ('Bella', 'Ramsey'),                             -- The Last of Us
('Leonardo', 'DiCaprio'), ('Brad', 'Pitt'),
('Millie', 'Bobby Brown'), ('Finn', 'Wolfhard')

-- 3. INSERIMENTO CONTENUTI (Film e Serie TV)
INSERT INTO Contents (ContentTitle, ReleaseYear, ContentType) VALUES 
('Oppenheimer', 2023, 'Movie'),         -- ID 1
('Breaking Bad', 2008, 'TV Series'),    -- ID 2
('The Last of Us', 2023, 'TV Series'),  -- ID 3
('C era una volta a Hollywood', 2019, 'Movie'),           -- ID 4
('Stranger Things', 2016, 'TV Series'); -- ID 5

-- 4. INSERIMENTO EPISODI (Solo per le Serie TV)
-- Breaking Bad (ContentID 2)
INSERT INTO Episodes (ContentID, SeasonNumber, EpisodeNumber, EpisodeTitle) VALUES 
(2, 1, 1, 'Pilot'),
(2, 1, 2, 'Cat''s in the Bag...'),
(2, 1, 3, '...And the Bag''s in the River'),
(2, 1, 4, 'Cancer Man'),
(2, 1, 5, 'Gray Matter'),
(2, 1, 6, 'Crazy Handful of Nothin'''),
(2, 1, 7, 'A No-Rough-Stuff-Type Deal');

-- 2. THE LAST OF US (ContentID 3) - Stagione 1
INSERT INTO Episodes (ContentID, SeasonNumber, EpisodeNumber, EpisodeTitle) VALUES 
(3, 1, 1, 'When You''re Lost in the Darkness'),
(3, 1, 2, 'Infected'),
(3, 1, 3, 'Long, Long Time'),
(3, 1, 4, 'Please Hold to My Hand'),
(3, 1, 5, 'Endure and Survive'),
(3, 1, 6, 'Kin'),
(3, 1, 7, 'Left Behind'),
(3, 1, 8, 'When We Are in Need'),
(3, 1, 9, 'Look for the Light');

-- 3. STRANGER THINGS (ContentID 5) - Stagione 1
INSERT INTO Episodes (ContentID, SeasonNumber, EpisodeNumber, EpisodeTitle) VALUES 
(5, 1, 1, 'Chapter One: The Vanishing of Will Byers'),
(5, 1, 2, 'Chapter Two: The Weirdo on Maple Street'),
(5, 1, 3, 'Chapter Three: Holly, Jolly'),
(5, 1, 4, 'Chapter Four: The Body'),
(5, 1, 5, 'Chapter Five: The Flea and the Acrobat'),
(5, 1, 6, 'Chapter Six: The Monster'),
(5, 1, 7, 'Chapter Seven: The Bathtub'),
(5, 1, 8, 'Chapter Eight: The Upside Down');
GO

-- 5. INSERIMENTO CAST (Relazione N:N)
INSERT INTO Casts (ActorID, ContentID) VALUES 
(1, 1), (2, 1), (3, 1), -- Cast Oppenheimer
(4, 2), (5, 2),         -- Cast Breaking Bad
(6, 3), (7, 3),         -- Cast The Last of Us
(8, 4), (9, 4),                 -- Cast Inception
(10, 5), (11, 5);

-- 6. INSERIMENTO UTENTI
INSERT INTO Users (UserName, UserSurname, UserEmail) VALUES 
('Mario', 'Rossi', 'mario.rossi@email.it'),
('Giulia', 'Bianchi', 'g.bianchi@provider.com'),
('Luca', 'Verdi', 'luca.verdi@test.it'),
('Elena', 'Neri', 'elena.neri@web.com');

-- 7. INSERIMENTO RECENSIONI
INSERT INTO Reviews (UserID, ContentID, Rating, Comment) VALUES 
(1, 1, 10, 'Capolavoro assoluto di Nolan.'),
(2, 1, 9, 'Recitazione incredibile, un po lungo ma ne vale la pena.'),
(3, 1, 5, 'Non rispecchia la realtà!'),
(1, 2, 10, 'La miglior serie TV di sempre.'),
(2, 2, 7, 'Nelle ultime stagioni si perde la trama'),
(1, 3, 10, 'Film incredibile, sembra di giocare al videogioco'),
(3, 3, 8, 'Ottimo adattamento del videogioco.'),
(4, 5, 7, 'Prima stagione fantastica, poi cala un po.'),
(2, 4, 7, 'Trama complessa e difficile da seguire.'),
(3, 4, 10, 'Tarantino non si smentisce mai');
GO