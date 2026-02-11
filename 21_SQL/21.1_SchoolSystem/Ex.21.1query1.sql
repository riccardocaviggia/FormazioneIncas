use SchoolSystemDB
select s.StudentID, s.StudentName, s.StudentSurname, s.StudentEmail, c.CourseName
from Students s
join Exams e on s.StudentID = e.StudentID join Courses c on e.CourseID = c.CourseID
where c.CourseName = 'Matematica avanzata'