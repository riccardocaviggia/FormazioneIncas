use StreamingPlatform 
select 
  c.ContentTitle, 
  e.SeasonNumber, 
  e.EpisodeNumber, 
  e.EpisodeTitle 
from 
  Episodes e 
  join Contents c on e.ContentID = c.ContentID 
where 
  c.ContentTitle = 'Breaking Bad' 
group by 
  c.ContentTitle, 
  e.SeasonNumber, 
  e.EpisodeNumber, 
  e.EpisodeTitle