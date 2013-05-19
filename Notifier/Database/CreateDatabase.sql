create table Contracts
(
	ID integer primary key autoincrement not null,
	ContractNumber nvarchar(100) unique not null,
	BorrowerName nvarchar(100) not null,
	PhoneNumber nvarchar(100),
	ExchangeRate decimal(18, 4) not null
);
go;

create table Payments
(
	ID integer primary key autoincrement not null,
	ContractID integer not null,
	PaymentDate date not null,
	PaymentAmount decimal(18, 4) not null,
	IsNotified boolean default false,
	foreign key(ContractID) references Contracts(ID)
);
go;
