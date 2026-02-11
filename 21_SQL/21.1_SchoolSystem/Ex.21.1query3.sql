use SchoolSystemDB 
select 
  t.TeacherName, 
  t.TeacherSurname, 
  count(c.CourseID) as number_of_courses 
from 
  Teachers t 
  join Courses c on t.TeacherID = c.TeacherID 
group by 
  t.TeacherID, 
  t.TeacherName, 
  t.TeacherSurname 
having 
  count(c.CourseID) > 2

