use MechanicalWorkShop 
select 
  w.WorkID, 
  w.Description, 
  w.WorkDate, 
  sum(r.ReplacementPrice * wd.Quantity) as replacements_total_cost 
from 
  Works w 
  join WorkDetails wd on w.WorkID = wd.WorkID 
  join Replacements r on wd.ReplacementID = r.ReplacementID 
group by 
  w.WorkID, 
  w.Description, 
  w.WorkDate
