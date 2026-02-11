use HospitalDB 
select 
  d.DoctorName, 
  d.DoctorSurname, 
  count(e.DoctorID) number_of_examinations 
from 
  Doctors d 
  join Examinations e on e.DoctorID = d.DoctorID 
group by 
  d.DoctorName, 
  d.DoctorSurname 
order by 
  number_of_examinations desc
