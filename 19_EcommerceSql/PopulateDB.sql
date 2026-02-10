use EcommerceDB
go

delete from Order_Details;
delete from Storage;
delete from Orders;
delete from Products;
delete from Clients;

DBCC CHECKIDENT ('Clients', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
GO

INSERT INTO Clients (ClientName, ClientSurname, ClientEmail) VALUES 
('Luca', 'Verdi', 'luca.verdi@gmail.com'),
('Giulia', 'Romano', 'giulia.romano@outlook.it'),
('Alessandro', 'Ferrari', 'a.ferrari@azienda.com'),
('Elena', 'Esposito', 'elena.espo@libero.it'),
('Roberto', 'Gallo', 'roberto.gallo@email.com');

INSERT INTO Products (ProductName, ProductPrice, ProductDescription) VALUES 
('Monitor Gaming 27', 349.99, '144Hz, 1ms, Pannello IPS'),
('Tastiera Meccanica', 89.50, 'Switch Cherry MX Red, RGB'),
('Webcam Full HD', 55.00, '1080p con microfono integrato'),
('SSD 1TB NVMe', 110.00, 'Velocità lettura 3500MB/s'),
('Zaino Porta PC', 45.00, 'Impermeabile con porta USB'),
('Mouse Pad XL', 15.99, 'Superficie in tessuto ottimizzata'),
('Cavo HDMI 2.1', 12.00, 'Supporto 8K 60Hz - 2 metri');

INSERT INTO Storage (ProductID, ProductQuantity) VALUES 
(1, 25), (2, 2), (3, 12), (4, 4), (5, 50), (6, 30), (7, 100);

INSERT INTO Orders (ClientID, OrderDate, OrderStatus, OrderPayment) VALUES 
(1, '2024-01-15 10:30:00', 'consegnato', 'Bonifico'),
(2, '2024-02-01 14:20:00', 'spedito', 'PayPal'), 
(3, '2024-02-09 09:00:00', 'in elaborazione', 'Carta'), 
(4, '2024-02-10 11:45:00', 'in elaborazione', 'Satispay'), 
(4, '2024-02-10 15:30:00', 'in elaborazione', 'Carta');

INSERT INTO Order_Details (OrderID, ProductID, ProductQuantity, PriceAtPurchase) VALUES 
(1, 1, 1, 349.99),
(1, 7, 2, 12.00),
(2, 2, 1, 89.50),
(3, 3, 1, 55.00),
(4, 4, 2, 110.00),
(4, 5, 1, 45.00),
(5, 1, 1, 349.99);
GO