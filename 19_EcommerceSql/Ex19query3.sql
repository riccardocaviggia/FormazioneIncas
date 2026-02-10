use EcommerceDB

select p.ProductName, sum(od.ProductQuantity) as products_sold
from Products p
join Order_Details od on od.ProductID = p.ProductID
group by p.ProductName