use EcommerceDB

select p.ProductName, s.ProductQuantity
from Products p
join Storage s on p.ProductID = s.ProductID
group by p.ProductName, s.ProductQuantity
having s.ProductQuantity < 5