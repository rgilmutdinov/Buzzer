drop view NotificationLogView;

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
inner join Persons as P on P.ID = NL.PersonID
where C.RowState <> 1;