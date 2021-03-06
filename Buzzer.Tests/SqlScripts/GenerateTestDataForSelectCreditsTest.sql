﻿CREATE TEMP TABLE ID (Value integer, CT1 integer, DT1 integer, DT2 integer);

-- Credit without Guarantors.
INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate, CreditState, ApplicationDate, ProtocolDate, RefusalReason, RowState)
VALUES
	('CNS1', 200000, '2014-01-02', 24, 0.36, 0.12, 47.5, 1, '2013-12-31', '2014-01-01', NULL, 0);
	
INSERT INTO ID VALUES ((SELECT last_insert_rowid()), 0, 0, 0);

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, PersonType)
VALUES
	((SELECT Value FROM ID LIMIT 1), '01234567890123', 'Borrower of CN2', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1);

-- Credit with Guarantors.
INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate, CreditState, ApplicationDate, ProtocolDate, RefusalReason, RowState)
VALUES
	('CNS2', 300000, '2014-01-02', 24, 0.36, NULL, NULL, 2, NULL, NULL, 'Refusal reason', 1);

UPDATE ID SET Value = (SELECT last_insert_rowid());

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, PersonType)
VALUES
	((SELECT Value FROM ID LIMIT 1), '12345678901234', 'Borrower of CN3', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1),
	((SELECT Value FROM ID LIMIT 1), '23456789012345', 'Guarantor 1 of CN3', 'Address 1', 'Fact address 1', 'Passport 1', '2013-12-02', 'Issuer 1', 0),
	((SELECT Value FROM ID LIMIT 1), '34567890123456', 'Guarantor 2 of CN3', 'Address 2', 'Fact address 2', 'Passport 2', '2013-12-03', 'Issuer 2', 0);

UPDATE ID SET Value = (SELECT last_insert_rowid());

INSERT INTO PhoneNumbers
	(PersonID, PhoneNumber)
VALUES
	((SELECT Value FROM ID LIMIT 1), '555666666'),
	((SELECT Value FROM ID LIMIT 1), '777888888');

-- Credit with PaymentsSchedule.
INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate, ApplicationDate, ProtocolDate)
VALUES
	('CNS3', 400000, '2014-01-25', 4, 0.36, NULL, NULL, NULL, NULL);

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

INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate, ApplicationDate, ProtocolDate)
VALUES
	('CNS4', 400000, '2014-01-25', 4, 0.36, NULL, NULL, NULL, NULL);
	
UPDATE ID SET Value = (SELECT last_insert_rowid());

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, PersonType)
VALUES
	((SELECT Value FROM ID LIMIT 1), '56789123450000', 'Borrower of CN4', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1);

INSERT INTO TodoItems
	(CreditID, Description, State, NotificationCount, NotificationDate)
VALUES
	((SELECT Value FROM ID LIMIT 1), 'Todo description 1', 0, 0, null),
	((SELECT Value FROM ID LIMIT 1), 'Todo description 2', 1, 1, '2015-01-24');

INSERT INTO DocumentTypes (Name) VALUES ('CNS5_DT1');
UPDATE ID SET DT1 = (SELECT last_insert_rowid());
INSERT INTO DocumentTypes (Name) VALUES ('CNS5_DT2');
UPDATE ID SET DT2 = (SELECT last_insert_rowid());

INSERT INTO CreditTypes (Name) VALUES ('CNS5_CT1');
UPDATE ID SET CT1 = (SELECT last_insert_rowid());

INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate,
	 ApplicationDate, ProtocolDate, CreditTypeID, NotificationDescription, NotificationCount, NotificationDate)
VALUES
	('CNS5', 500000, '2015-02-07', 12, 0.36, NULL, NULL, NULL, NULL,
	 (SELECT CT1 FROM ID LIMIT 1), 'Description', 1, '2015-02-07');

UPDATE ID SET Value = (SELECT last_insert_rowid());

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, PersonType)
VALUES
	((SELECT Value FROM ID LIMIT 1), '12345884521657', 'Borrower of CSN5', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1);

INSERT INTO RequiredDocuments
	(CreditID, DocumentTypeID, State)
VALUES
	((SELECT Value FROM ID LIMIT 1), (SELECT DT1 FROM ID LIMIT 1), 0),
	((SELECT Value FROM ID LIMIT 1), (SELECT DT2 FROM ID LIMIT 1), 1);

DROP TABLE ID;
