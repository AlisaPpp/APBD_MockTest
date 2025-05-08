INSERT INTO Currency (Name, Rate) VALUES ('USD', 1.0);
INSERT INTO Currency (Name, Rate) VALUES ('EUR', 0.89);
INSERT INTO Currency (Name, Rate) VALUES ('JPY', 145.87);
INSERT INTO Currency (Name, Rate) VALUES ('PLN', 3.79);

INSERT INTO Country (Name) VALUES ('United States');
INSERT INTO Country (Name) VALUES ('France');
INSERT INTO Country (Name) VALUES ('Greece');
INSERT INTO Country (Name) VALUES ('Puerto Rico');
INSERT INTO Country (Name) VALUES ('Japan');
INSERT INTO Country (Name) VALUES ('Poland');
INSERT INTO Country (Name) VALUES ('Spain');


INSERT INTO Currency_Country (Currency_Id, Country_Id) VALUES (1, 1);
INSERT INTO Currency_Country (Currency_Id, Country_Id) VALUES (1, 4);
INSERT INTO Currency_Country (Currency_Id, Country_Id) VALUES (2, 2);
INSERT INTO Currency_Country (Currency_Id, Country_Id) VALUES (2, 3);
INSERT INTO Currency_Country (Currency_Id, Country_Id) VALUES (3, 5);
INSERT INTO Currency_Country (Currency_Id, Country_Id) VALUES (4, 6);

SELECT * FROM Currency WHERE Id = 4;
SELECT * FROM Currency_Country WHERE Currency_Id = 4 AND Country_Id = 6;
SELECT c.Name, c.Rate
FROM Currency c
         JOIN Currency_Country cc ON c.Id = cc.Currency_Id
WHERE cc.Country_Id = (SELECT Id FROM Country WHERE Name = 'Poland');










