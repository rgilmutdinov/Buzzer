DECLARE @ID AS BIGINT;

INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate)
VALUES
	('CNE1', 100000, '2013-12-31', 12, 0.36, 0.24, 45);
	
SET @ID = (SELECT SCOPE_IDENTITY());

INSERT INTO Persons
	(CreditID, PersonalNumber, Name, RegistrationAddress, FactAddress, PassportNumber, PassportIssueDate, PassportIssuer, IsBorrower)
VALUES
	(@ID, '11111111111111', 'Borrower', 'Address', 'Fact address', 'Passport', '2013-12-01', 'Issuer', 1),
	(@ID, '22222222222222', 'Guarantor 1', 'Address 1', 'Fact address 1', 'Passport 1', '2013-12-02', 'Issuer 1', 0),
	(@ID, '33333333333333', 'Guarantor 2', 'Address 2', 'Fact address 2', 'Passport 2', '2013-12-03', 'Issuer 2', 0);

SET @ID = (SELECT SCOPE_IDENTITY());

INSERT INTO PhoneNumbers
	(PersonID, PhoneNumber)
VALUES
	(@ID, '111111111'),
	(@ID, '222222222');

-- Credit with PaymentsSchedule.
INSERT INTO Credits
	(CreditNumber, CreditAmount, CreditIssueDate, MonthsCount, DiscountRate, EffectiveDiscountRate, ExchangeRate)
VALUES
	('CNE2', 400000, '2014-01-25', 4, 0.36, NULL, NULL);

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
