create table CreditTypes
(
	ID integer primary key autoincrement not null,
	Name nvarchar(255)
);

create table DocumentTypes
(
	ID integer primary key autoincrement not null,
	Name nvarchar(255)
);

create table RequiredCreditDocuments
(
	ID integer primary key autoincrement not null,
	CreditTypeID integer not null,
	DocumentTypeID integer not null,
	foreign key(CreditTypeID) references CreditTypes(ID),
	foreign key(DocumentTypeID) references DocumentTypes(ID)
);
