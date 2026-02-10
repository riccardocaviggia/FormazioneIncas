use EcommerceDB

select p.ProductName, sum(od.ProductQuantity) as product_quantity
from Products p
join Order_Details od on od.ProductID = p.ProductID
group by p.ProductName