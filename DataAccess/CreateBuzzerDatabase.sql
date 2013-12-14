create table Credits
(
	ID integer primary key identity(1, 1) not null,
	CreditNumber nvarchar(100) unique not null,
	CreditAmount decimal not null,
	CreditIssueDate date not null,
	MonthsCount integer not null,
	DiscountRate decimal(18, 4) not null,
	EffectiveDiscountRate decimal(18, 4),
	ExchangeRate decimal(18, 4)
);
go

create table Persons
(
	ID integer primary key identity(1, 1) not null,
	CreditID integer not null references Credits(ID),
	PersonalNumber nvarchar(100) not null,
	Name nvarchar(255) not null,
	RegistrationAddress nvarchar(255) not null,
	FactAddress nvarchar(255) not null,
	PassportNumber nvarchar(100) not null,
	PassportIssueDate date not null,
	PassportIssuer nvarchar(100) not null,
	IsBorrower bit not null
);
go

create table PhoneNumbers
(
	ID integer primary key identity(1, 1) not null,
	PersonID integer not null references Persons(ID),
	PhoneNumber nvarchar(100) not null
);
go