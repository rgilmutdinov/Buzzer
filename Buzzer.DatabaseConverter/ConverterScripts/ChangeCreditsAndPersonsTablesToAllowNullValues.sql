-- Recreate Credits table.
create table Credits2
(
	ID integer primary key autoincrement not null,
	CreditState integer not null default 1,
	CreditNumber nvarchar(100),
	CreditAmount decimal not null,
	CreditIssueDate date not null,
	MonthsCount integer not null,
	DiscountRate decimal(18, 4) not null,
	EffectiveDiscountRate decimal(18, 4),
	ExchangeRate decimal(18, 4)
);

insert into Credits2
(
	ID, CreditState, CreditNumber, CreditAmount, CreditIssueDate,
	MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate
)
select
	ID, CreditState, CreditNumber, CreditAmount, CreditIssueDate,
	MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate
from Credits;

drop table Credits;
alter table Credits2 rename to Credits;

-- Recreate Persons table.
create table Persons2
(
	ID integer primary key autoincrement not null,
	CreditID integer not null,
	PersonalNumber nvarchar(100),
	Name nvarchar(255),
	RegistrationAddress nvarchar(255),
	FactAddress nvarchar(255),
	PassportNumber nvarchar(100),
	PassportIssueDate date not null,
	PassportIssuer nvarchar(100),
	PersonType integer not null,
	foreign key(CreditID) references Credits(ID)
);

insert into Persons2
(
	ID, CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress,
	PassportNumber, PassportIssueDate, PassportIssuer, PersonType
)
select
	ID, CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress,
	PassportNumber, PassportIssueDate, PassportIssuer, IsBorrower
from Persons;

drop table Persons;
alter table Persons2 rename to Persons;