CREATE TEMP TABLE ID (Value integer, CT1 integer, CT2 integer, DT1 integer, DT2 integer, DT3 integer);

INSERT INTO CreditTypes (Name) VALUES ('SC_CT1');
INSERT INTO ID (Value, CT1, CT2, DT1, DT2, DT3) VALUES (0, (SELECT last_insert_rowid()), 0, 0, 0, 0);

INSERT INTO CreditTypes (Name) VALUES ('SC_CT2');
UPDATE ID SET CT2 = (SELECT last_insert_rowid());

INSERT INTO DocumentTypes (Name) VALUES ('SC_DT1');
UPDATE ID SET DT1 = (SELECT last_insert_rowid());

INSERT INTO DocumentTypes (Name) VALUES ('SC_DT2');
UPDATE ID SET DT2 = (SELECT last_insert_rowid());

INSERT INTO DocumentTypes (Name) VALUES ('SC_DT3');
UPDATE ID SET DT3 = (SELECT last_insert_rowid());

INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate, CreditState,
	 CreditTypeID, NotificationDescription, NotificationCount, NotificationDate)
VALUES
	('CNE1', 100000, '2013-12-31', 12, 0.36, 0.24, 45, 1, 
	 (SELECT CT2 FROM ID LIMIT 1), 'Notification description', 1, '2015-02-06');

UPDATE ID SET Value = (SELECT last_insert_rowid());

INSERT INTO TodoItems
	(CreditID, Description, State, NotificationCount, NotificationDate)
VALUES
	((SELECT Value FROM ID LIMIT 1), 'Todo item 1', 0, 0, null),
	((SELECT Value FROM ID LIMIT 1), 'Todo item 2', 0, 0, null);

INSERT INTO RequiredDocuments
	(CreditID, DocumentTypeID, State)
VALUES
	((SELECT Value FROM ID LIMIT 1), (SELECT DT1 FROM ID LIMIT 1), 0),
	((SELECT Value FROM ID LIMIT 1), (SELECT DT2 FROM ID LIMIT 1), 0);

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, PersonType)
VALUES
	((SELECT Value FROM ID LIMIT 1), '11111111111111', 'Borrower', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1),
	((SELECT Value FROM ID LIMIT 1), '22222222222222', 'Guarantor 1', 'Address 1', 'Fact address 1', 'Passport 1', '2013-12-02', 'Issuer 1', 0),
	((SELECT Value FROM ID LIMIT 1), '33333333333333', 'Guarantor 2', 'Address 2', 'Fact address 2', 'Passport 2', '2013-12-03', 'Issuer 2', 0);

UPDATE ID SET Value = (SELECT last_insert_rowid());

INSERT INTO PhoneNumbers
	(PersonID, PhoneNumber)
VALUES
	((SELECT Value FROM ID LIMIT 1), '111111111'),
	((SELECT Value FROM ID LIMIT 1), '222222222');

-- Credit with PaymentsSchedule.
INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate)
VALUES
	('CNE2', 400000, '2014-01-25', 4, 0.36, NULL, NULL);

UPDATE ID SET Value = (SELECT last_insert_rowid());

INSERT INTO PaymentsSchedule
	(CreditID, PaymentDate, PaymentAmount, IsNotified)
VALUES
	((SELECT Value FROM ID LIMIT 1), '2014-02-25', 107851, 1),
	((SELECT Value FROM ID LIMIT 1), '2014-03-25', 107793, 1),
	((SELECT Value FROM ID LIMIT 1), '2014-04-25', 107734, 0),
	((SELECT Value FROM ID LIMIT 1), '2014-05-25', 107674, 0);

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, PersonType)
VALUES
	((SELECT Value FROM ID LIMIT 1), '12345678901234', 'Borrower of CN4', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1),
	((SELECT Value FROM ID LIMIT 1), '23456789012345', 'Guarantor of CN4', 'Address', 'Fact address', 'Passport', '2013-12-02', 'Issuer', 0);

DROP TABLE ID;