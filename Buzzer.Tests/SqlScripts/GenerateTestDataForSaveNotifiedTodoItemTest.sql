CREATE TEMP TABLE ID (CreditID integer);

INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate)
VALUES
	('CNSNTI', 100000, '2015-01-25', 12, 0.36, 0.24, 60);

INSERT INTO ID (CreditID) VALUES ((SELECT last_insert_rowid()));

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, PersonType)
VALUES
	((SELECT CreditID FROM ID LIMIT 1), '11111111111111', 'Borrower', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1);

INSERT INTO TodoItems
	(CreditID, Description, State, NotificationCount, NotificationDate)
VALUES
	((SELECT CreditID FROM ID LIMIT 1), 'Todo item to update', 0, 0, NULL);

DROP TABLE ID;