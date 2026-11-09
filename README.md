# DocumentManagement_Enriquez_SeanMcHalle
*Project Ovewiew
this project is backend project made from ASP.NET Core Web API using C# and .NET8
with the use of Dapper to connect and manilate data to SQL Server Database
This project consist of end point to Upload, Retrive all and specific data, delete, and download file 

*Database Setup
-Add connection on appsettings.json
-Run sql file to create Database, Table, and Stored Procedure file

*How to use and test
-This project can be tested by running the visual studio and using the swagger to pass a request
unit test can be also run using visula studio by clickin the RunAllTest in the Menu test

*Implementation End point
-POST/api/documents
-GET/api/documents
-GET/api/documents/{id}
-GET/api/{id}/download
-DELETE/api/documents/{id}


*Chosen Implementation
-For this exam I chose dapper to connect and manipulate data from sql server to my web api project
then for the architecture and pattern, I just organized my project by creating folder to separate data layer and business layer just to see clean look structure 
and since it is not required on the instruction, I did not create library to separate the ntier layer. 

for business layer I have a class that serve as a service where the logic and validation happens and
repository class to separate the database manipulation


Again, Thank you for considering my application and I hope I get an technical interview to talk further about my skills and experiences


