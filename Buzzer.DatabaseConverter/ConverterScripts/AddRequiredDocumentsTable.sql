create table RequiredDocuments
(
	ID integer primary key autoincrement not null,
	CreditID integer not null,
	DocumentTypeID not null,
	State integer not null,
	foreign key(CreditID) references Credits(ID),
	foreign key(DocumentTypeID) references DocumentTypes(ID)
);

alter table Credits add column CreditTypeID integer null references CreditTypes(ID);
alter table Credits add column NotificationDescription nvarchar(2000) null;
alter table Credits add column NotificationCount integer not null default 0;
alter table Credits add column NotificationDate date null;