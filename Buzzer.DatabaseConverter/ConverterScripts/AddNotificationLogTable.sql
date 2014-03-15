create table NotificationLog
(
	ID integer primary key autoincrement not null,
	CreditID integer not null,
	PersonID integer not null,
	NotificationDate date not null,
	Comment nvarchar(2000),
	foreign key(CreditID) references Credits(ID),
	foreign key(PersonID) references Persons(ID)
);

create view NotificationLogView as
select
	NL.ID as ID,
	C.CreditNumber as CreditNumber,
	P.Name as PersonName,
	NL.CreditID as CreditID,
	NL.PersonID as PersonID,
	NL.NotificationDate as NotificationDate,
	NL.Comment as Comment
from NotificationLog as NL
inner join Credits as C on C.ID = NL.CreditID
inner join Persons as P on P.ID = NL.PersonID;