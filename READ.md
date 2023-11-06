In this Assessment we require you to build an application that does the following:
Develop a .net 6+ backend Web API that connects to a SQL database (ORM of your choice)
In the backend project please use whatever technology you prefer but sticking to the .net 6+ as the
base of the project.
Requirements in summary:
• Build the backend in .net 6+
• Apply SOLID principles to your code
• Create web APIs that can do the following:
o Upload and validate a CSV file (Example of the CSV below, Rules and requirements
and more info to follow)




1) Run SQL  Create a database  Nameed CSVassessment  by running the below command: 
-CREATE DATABASE CSVassessment;

2) Create the "CSV_data" table with the below command: 

CREATE TABLE CSV_data (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255),
    Type NVARCHAR(255),
    Search NVARCHAR(10),
    "Library Filter" NVARCHAR(10),
    Visible NVARCHAR(10)
);