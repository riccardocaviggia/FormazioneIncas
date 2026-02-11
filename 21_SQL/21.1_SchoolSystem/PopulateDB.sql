USE SchoolSystemDB
GO

DELETE FROM Exams;
DELETE FROM Courses;
DELETE FROM Students;
DELETE FROM Teachers;

DBCC CHECKIDENT ('Teachers', RESEED, 0);
DBCC CHECKIDENT ('Students', RESEED, 0);
DBCC CHECKIDENT ('Courses', RESEED, 0);
DBCC CHECKIDENT ('Exams', RESEED, 0);
GO

INSERT INTO Teachers (TeacherName, TeacherSurname, TeacherEmail) VALUES 
('Roberto', 'Verdi', 'r.verdi@scuola.it'),
('Anna', 'Neri', 'a.neri@scuola.it'),
('Paolo', 'Rossi', 'p.rossi@scuola.it'),
('Lucia', 'Gallo', 'l.gallo@scuola.it'),
('Marco', 'Fermi', 'm.fermi@scuola.it');

INSERT INTO Students (StudentName, StudentSurname, StudentEmail) VALUES 
('Mario', 'Rossi', 'mario.rossi@studente.it'),
('Elena', 'Bianchi', 'elena.bianchi@studente.it'),
('Luca', 'Gialli', 'luca.gialli@studente.it'),
('Sofia', 'Romano', 'sofia.romano@studente.it'),
('Matteo', 'Ricci', 'm.ricci@studente.it'),
('Chiara', 'Moretti', 'c.moretti@studente.it'),
('Davide', 'Bruno', 'd.bruno@studente.it'),
('Alice', 'Marino', 'a.marino@studente.it'),
('Gabriele', 'Lombardi', 'g.lombardi@studente.it'),
('Sara', 'Barbieri', 's.barbieri@studente.it');

INSERT INTO Courses (CourseName, TeacherID) VALUES 
('Matematica Avanzata', 1),
('Fisica Nucleare', 1), 
('Storia Medievale', 2),   
('Storia Contemporanea', 2),  
('Storia Moderna', 2),
('Letteratura Italiana', 3), 
('Filosofia Estetica', 3),  
('Informatica 101', 4),    
('Chimica Organica', 5);     

INSERT INTO Exams (CourseID, StudentID, ExamGrade, ExamDate) VALUES 
(1, 1, 28, '2025-05-10'), (1, 2, 25, '2025-05-10'), (1, 3, 30, '2025-05-10'),
(2, 1, 30, '2025-06-15'), (2, 5, 27, '2025-06-15'),
(3, 2, 30, '2025-05-20'), (3, 4, 24, '2025-05-20'), (3, 6, 26, '2025-05-20'),
(4, 7, 18, '2025-07-01'), (4, 8, 22, '2025-07-01'),
(6, 1, 29, '2025-05-25'), (6, 9, 30, '2025-05-25'), (6, 10, 25, '2025-05-25'),
(7, 2, 28, '2025-06-05'), (7, 4, 30, '2025-06-05'),
(8, 3, 30, '2025-05-12'), (8, 5, 28, '2025-05-12'), (8, 7, 24, '2025-05-12'),
(9, 8, 26, '2025-06-20'), (9, 9, 21, '2025-06-20'), (9, 10, 23, '2025-06-20');
GO