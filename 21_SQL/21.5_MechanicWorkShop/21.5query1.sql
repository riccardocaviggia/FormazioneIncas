use MechanicalWorkShop 
select 
  v.VehiclePlate, 
  v.VehicleBrand, 
  w.WorkDate, 
  w.Description, 
  w.WorkCost 
from 
  Works w 
  join Vehicles v on w.VehicleID = v.VehicleID 
where 
  v.VehicleBrand = 'Fiat'
