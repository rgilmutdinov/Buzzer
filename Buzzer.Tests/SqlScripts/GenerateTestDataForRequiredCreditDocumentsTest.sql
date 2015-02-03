CREATE TEMP TABLE ID (DT1 integer, DT2 integer, CT1 integer, CT2 integer, CT4 integer, CT5 integer);

INSERT INTO DocumentTypes (Name) VALUES ('DT1');
INSERT INTO ID (DT1) VALUES ((SELECT last_insert_rowid()));

INSERT INTO DocumentTypes (Name) VALUES ('DT2');
UPDATE ID SET DT2 = (SELECT last_insert_rowid());

INSERT INTO DocumentTypes (Name) VALUES ('DT3');

INSERT INTO CreditTypes (Name) VALUES ('CT1');
UPDATE ID SET CT1 = (SELECT last_insert_rowid());

INSERT INTO CreditTypes (Name) VALUES ('CT2');
UPDATE ID SET CT2 = (SELECT last_insert_rowid());

INSERT INTO CreditTypes (Name) VALUES ('CT3');

INSERT INTO CreditTypes (Name) VALUES ('CT4');
UPDATE ID SET CT4 = (SELECT last_insert_rowid());

INSERT INTO CreditTypes (Name) VALUES ('CT5');
UPDATE ID SET CT5 = (SELECT last_insert_rowid());

INSERT INTO RequiredCreditDocuments
	(CreditTypeID, DocumentTypeID)
VALUES
	((SELECT CT1 FROM ID LIMIT 1), (SELECT DT1 FROM ID LIMIT 1)),
	((SELECT CT2 FROM ID LIMIT 1), (SELECT DT1 FROM ID LIMIT 1)),
	((SELECT CT2 FROM ID LIMIT 1), (SELECT DT2 FROM ID LIMIT 1)),
	((SELECT CT4 FROM ID LIMIT 1), (SELECT DT1 FROM ID LIMIT 1)),
	((SELECT CT4 FROM ID LIMIT 1), (SELECT DT2 FROM ID LIMIT 1)),
	((SELECT CT5 FROM ID LIMIT 1), (SELECT DT1 FROM ID LIMIT 1));

DROP TABLE ID;