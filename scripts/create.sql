CREATE TABLE Currency (
            Id INT PRIMARY KEY IDENTITY (1, 1),
            Name NVARCHAR(30),
            Rate FLOAT(2)  
);

CREATE TABLE Country (
            Id INT PRIMARY KEY IDENTITY (1, 1),
            Name NVARCHAR(80)
);

CREATE TABLE Currency_Country (
            Currency_Id INT,
            Country_Id INT,
            PRIMARY KEY (Currency_Id, Country_Id),
            FOREIGN KEY (Currency_Id) REFERENCES Currency(Id),
            FOREIGN KEY (Country_Id) REFERENCES Country(Id)
);
