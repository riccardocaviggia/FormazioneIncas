USE HospitalDB
GO

DELETE FROM Therapies;
DELETE FROM Examinations;
DELETE FROM Patients;
DELETE FROM Doctors;
DELETE FROM Departments;

DBCC CHECKIDENT ('Departments', RESEED, 0);
DBCC CHECKIDENT ('Doctors', RESEED, 0);
DBCC CHECKIDENT ('Patients', RESEED, 0);
DBCC CHECKIDENT ('Examinations', RESEED, 0);
DBCC CHECKIDENT ('Therapies', RESEED, 0);
GO

INSERT INTO Departments (DepartmentName) VALUES 
('Cardiologia'),
('Ortopedia'),
('Pediatria'),
('Neurologia');

INSERT INTO Doctors (DoctorName, DoctorSurname, DepartmentID, DoctorSpecialization) VALUES 
('Mario', 'Rossi', 1, 'Cardiologo'),
('Elena', 'Bianchi', 1, 'Aritmologo'),
('Luca', 'Verdi', 2, 'Traumatologo'),
('Giulia', 'Neri', 3, 'Pediatra'),
('Paolo', 'Bruni', 4, 'Neurologo');

INSERT INTO Patients (PatientName, PatientSurname) VALUES 
('Alessandro', 'Gallo'),
('Sofia', 'Russo'),
('Matteo', 'Ferrari'),
('Chiara', 'Esposito'),
('Davide', 'Romano');

INSERT INTO Examinations (DoctorID, PatientID, ExaminationDate, ExaminationDateFinish, ExaminationReason) VALUES 
(1, 1, '2025-02-10 09:30:00', NULL, 'Controllo post-operatorio'),
(3, 1, '2025-02-11 11:00:00', NULL, 'Dolore persistente ginocchio'),
(4, 2, '2025-02-11 15:45:00', '2025-03-15', 'Visita pediatrica di routine'),
(5, 3, '2025-02-12 10:00:00', NULL, 'Emicrania cronica'),
(2, 4, GETDATE(), NULL, 'Elettrocardiogramma'),
(3, 3, '2025-03-01 09:15:00', '2025-04-13', 'Visita posturale');

INSERT INTO Therapies (PatientID, TherapyDescription, TherapyEndDate) VALUES 
(1, 'Riabilitazione motoria 2 volte a settimana', '2025-05-20'),
(3, 'Terapia farmacologica per cefalea', '2025-03-15'),
(5, 'Monitoraggio pressione arteriosa', '2025-08-30');
GO