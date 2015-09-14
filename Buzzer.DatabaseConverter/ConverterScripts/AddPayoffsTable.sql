create table Payoffs
(
	ID integer primary key autoincrement not null,
	CreditID integer not null,
	PayoffDate date not null,
	PayoffAmount decimal not null,
	Remarks nvarchar(2000),
	foreign key(CreditID) references Credits(ID)
);