create table TodoItems
(
	ID integer primary key autoincrement not null,
	CreditID integer not null,
	Description nvarchar(255) not null,
	State integer not null,
	NotificationCount integer not null default 0,
	NotificationDate date,
	foreign key(CreditID) references Credits(ID)
);