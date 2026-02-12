use MechanicalWorkShop 
select 
  m.MechanicName, 
  m.MechanicSurname, 
  count(w.WorkID) number_of_repairs 
from 
  Mechanics m 
  join Works w on m.MechanicID = w.MechanicID 
group by 
  m.MechanicID, 
  m.MechanicName, 
  m.MechanicSurname 
having 
  count(w.WorkID) > 2
