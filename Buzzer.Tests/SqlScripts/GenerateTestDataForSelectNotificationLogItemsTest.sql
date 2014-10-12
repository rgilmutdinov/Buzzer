CREATE TEMP TABLE ID (CreditID integer, PersonID integer);

INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate)
VALUES
	('CNSNLI1', 100000, '2013-12-31', 12, 0.36, 0.24, 45);

INSERT INTO ID (CreditID, PersonID) VALUES ((SELECT last_insert_rowid()), 0);

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, PersonType)
VALUES
	((SELECT CreditID FROM ID LIMIT 1), '11111111111111', 'Borrower', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1);

UPDATE ID SET PersonID = (SELECT last_insert_rowid());

INSERT INTO NotificationLog (CreditID, PersonID, NotificationDate, Comment)
VALUES ((SELECT CreditID FROM ID LIMIT 1), (SELECT PersonID FROM ID LIMIT 1), '2013-12-31 10:20:30', NULL);

INSERT INTO NotificationLog (CreditID, PersonID, NotificationDate, Comment)
VALUES ((SELECT CreditID FROM ID LIMIT 1), (SELECT PersonID FROM ID LIMIT 1), '2014-01-15 10:20:30', 'Comment');

INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate, RowState)
VALUES
	('CNSDNLI1', 100000, '2013-12-31', 12, 0.36, 0.24, 45, 1);

UPDATE ID SET CreditID = (SELECT last_insert_rowid());

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, PersonType)
VALUES
	((SELECT CreditID FROM ID LIMIT 1), '12121212121212', 'Borrower of deleted credit contract', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1);

UPDATE ID SET PersonID = (SELECT last_insert_rowid());

INSERT INTO NotificationLog (CreditID, PersonID, NotificationDate, Comment)
VALUES ((SELECT CreditID FROM ID LIMIT 1), (SELECT PersonID FROM ID LIMIT 1), '2013-12-31 10:20:30', 'Notification log item of the deleted credit contract');

DROP TABLE ID;