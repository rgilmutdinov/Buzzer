create table Credits
(
	ID integer primary key autoincrement not null,
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
	ID integer primary key autoincrement not null,
	CreditID integer not null,
	PersonalNumber nvarchar(100) not null,
	Name nvarchar(255) not null,
	RegistrationAddress nvarchar(255) not null,
	FactAddress nvarchar(255) not null,
	PassportNumber nvarchar(100) not null,
	PassportIssueDate date not null,
	PassportIssuer nvarchar(100) not null,
	IsBorrower bit not null,
	foreign key(CreditID) references Credits(ID)
);
go

create table PhoneNumbers
(
	ID integer primary key autoincrement not null,
	PersonID integer not null,
	PhoneNumber nvarchar(100) not null,
	foreign key(PersonID) references Persons(ID)
);
go

create table PaymentsSchedule
(
	ID integer primary key autoincrement not null,
	CreditID integer not null,
	PaymentDate date not null,
	PaymentAmount decimal not null,
	IsNotified bit default 0 not null,
	foreign key(CreditID) references Credits(ID)
);
go
