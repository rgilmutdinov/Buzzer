DECLARE @ID AS BIGINT;

-- Credit without Guarantors.
INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate)
VALUES
	('CNS1', 200000, '2014-01-02', 24, 0.36, 0.12, 47.5);
	
SET @ID = (SELECT SCOPE_IDENTITY());

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, IsBorrower)
VALUES
	(@ID, '01234567890123', 'Borrower of CN2', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1);

-- Credit with Guarantors.
INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate)
VALUES
	('CNS2', 300000, '2014-01-02', 24, 0.36, NULL, NULL);

SET @ID = (SELECT SCOPE_IDENTITY());

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, IsBorrower)
VALUES
	(@ID, '12345678901234', 'Borrower of CN3', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1),
	(@ID, '23456789012345', 'Guarantor 1 of CN3', 'Address 1', 'Fact address 1', 'Passport 1', '2013-12-02', 'Issuer 1', 0),
	(@ID, '34567890123456', 'Guarantor 2 of CN3', 'Address 2', 'Fact address 2', 'Passport 2', '2013-12-03', 'Issuer 2', 0);

SET @ID = (SELECT SCOPE_IDENTITY());

INSERT INTO PhoneNumbers
	(PersonID, PhoneNumber)
VALUES
	(@ID, '555666666'),
	(@ID, '777888888');

-- Credit with PaymentsSchedule.
INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate)
VALUES
	('CNS3', 400000, '2014-01-25', 4, 0.36, NULL, NULL);

SET @ID = (SELECT SCOPE_IDENTITY());

INSERT INTO PaymentsSchedule
	(CreditID, PaymentDate, PaymentAmount, IsNotified)
VALUES
	(@ID, '2014-02-25', 107851, 1),
	(@ID, '2014-03-25', 107793, 1),
	(@ID, '2014-04-25', 107734, 0),
	(@ID, '2014-05-25', 107674, 0);

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, IsBorrower)
VALUES
	(@ID, '12345678901234', 'Borrower of CN4', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1),
	(@ID, '23456789012345', 'Guarantor of CN4', 'Address', 'Fact address', 'Passport', '2013-12-02', 'Issuer', 0);
