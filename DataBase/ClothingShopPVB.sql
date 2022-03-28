create database ClothingShopPVB
go
use ClothingShopPVB
go

create table Category
(
	Cat_Id int identity(1,1) primary key,
	Cat_Name varchar(100),

)
go


create table Product 
(
	Product_ID int identity(1,1) primary key,
	Product_Name varchar(225) not null,
	Product_Image varchar(50) not null,
	Product_Price float not null,
	Product_SalePrice float not null,
	Product_Quantity int not null,
	Product_Discription varchar(max),
	ImprortDay smalldatetime,
	UpdateDay smalldatetime,
	Product_Status bit,
	Cat_Id int foreign key references Category(Cat_Id)



)
go

create table Roles
(
	Roles_Id int identity(1,1) primary key,
	Roles_name varchar(50) not null
)
go

create table Users
(
	Users_Id int identity primary key,
	Users_Email varchar(50) not null, --Dùng để làm tên đăng nhập
	Fullname varchar(225) not null,
	Users_BirthDay smalldatetime not null,
	Users_Address varchar(100) not null,
	Users_Phone char(15) not null,

	Roles_Id int foreign key references Roles(Roles_Id)


)
go

create table Orders
(
	Orders_Id int identity primary key,
	Orders_Date smalldatetime not null,
	total int not null,
	Users_Id int foreign key references Users(Users_Id)
)
go

create table OrderDetails
(
	OrderDetails_id int identity primary key,
	Orders_Id int foreign key references Users(Users_Id),
	Product_ID int foreign key references Product(Product_ID),
	OrderDetails_Quantity int not null,
	OrderDetails_SalePrice int 
)
go
