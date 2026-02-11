use StreamingPlatform 
select 
  c.ContentTitle, 
  avg(
    cast(
      r.Rating as decimal(10, 2)
    )
  ) as movie_rating 
from 
  Contents c 
  join Reviews r on r.ContentID = c.ContentID 
group by 
  c.ContentTitle