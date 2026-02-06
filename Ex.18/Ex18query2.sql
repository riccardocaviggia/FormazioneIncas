select u.UserName, u.UserSurname
from Users u
join BookLoans bl on u.UserID = bl.UserID
group by u.UserName, u.UserSurname
having count(bl.UserID) > 3