select c.CourseID, c.CourseName, avg(e.ExamGrade) as average_grade
from Courses c
join Exams e on e.CourseID = c.CourseID
group by c.CourseID, c.CourseName