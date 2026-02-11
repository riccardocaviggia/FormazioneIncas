use StreamingPlatform 
select 
  a.ActorName, 
  a.ActorSurname, 
  count(c.ContentID) as number_of_films 
from 
  Actors a 
  join Casts cs on a.ActorID = cs.ActorID 
  join Contents c on cs.ContentID = c.ContentID 
where 
  c.ContentType = 'Movie' 
group by 
  a.ActorID, 
  a.ActorName, 
  a.ActorSurname 
having 
  count(c.ContentID) > 5