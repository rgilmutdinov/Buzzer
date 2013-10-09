create table Persons
(
	ID integer primary key identity(1, 1) not null,
	Name nvarchar(255) not null,
	RegistrationAddress nvarchar(255),
	FactAddress nvarchar(255),
	PassportNumber nvarchar(100),
	PassportIssueDate date,
	PassportIssuer nvarchar(100),
	IsValid bit not null
);
go

create table PhoneNumbers
(
	ID integer primary key identity(1, 1) not null,
	PersonID integer not null references Persons(ID),
	PhoneNumber nvarchar(100) not null
);
go

create table Credits
(
	ID integer primary key identity(1, 1) not null,
	CreditNumber nvarchar(100) unique not null,
	CreditAmount decimal,
	CreditIssueDate date,
	MonthsCount integer,
	DiscountRate decimal(18, 4),
	EffectiveDiscountRate decimal(18, 4),
	ExchangeRate decimal(18, 4)
);
go

create table PersonsToCredits
(
	CreditID integer not null references Credits(ID),
	PersonID integer not null references Persons(ID),
	IsBorrower bit not null
);
go