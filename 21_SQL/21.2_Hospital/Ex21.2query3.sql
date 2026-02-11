use HospitalDB 
select 
  p.PatientName, 
  p.PatientSurname, 
  e.ExaminationReason, 
  DATEDIFF(
    day, e.ExaminationDate, e.ExaminationDateFinish
  ) as examination_duration 
from 
  Patients p 
  join Examinations e on e.PatientID = p.PatientID 
where 
  DATEDIFF(
    day, e.ExaminationDate, e.ExaminationDateFinish
  ) > 30
