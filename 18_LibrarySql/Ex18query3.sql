use LibraryDB
select a.AuthorName, a.AuthorSurname, count(ba.BookID) as books_written
from Authors a
join BookAuthors ba on a.AuthorID = ba.AuthorID
group by a.AuthorName, a.AuthorSurname
order by books_written desc