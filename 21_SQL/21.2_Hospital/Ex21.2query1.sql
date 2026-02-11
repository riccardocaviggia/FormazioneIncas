use HospitalDB 
select 
  p.PatientName, 
  p.PatientSurname, 
  e.ExaminationReason 
from 
  Patients p 
  join Examinations e on e.PatientID = p.PatientID 
  join Doctors d on e.DoctorID = d.DoctorID 
  join Departments dp on d.DepartmentID = dp.DepartmentID 
where 
  dp.DepartmentName = 'Cardiologia' 
group by 
  p.PatientName, 
  p.PatientSurname, 
  e.ExaminationReason
