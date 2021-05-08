# SqGoods
It is a demo web application that demonstrates abilities of [SqExpress](https://github.com/0x1000000/SqExpress) and [ngSetState](https://github.com/0x1000000/ngSetState) libraries.

SqGoods it is a catalog of products with dynamic attributes. 

The key feature is a complex filtering (any kind of boolean expressions) using the dynamic attributes. 

## Prerequisites
- [.Net 5 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/5.0)
- [.Net Core 3.1 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/3.1) (to run the code-generation)
- NodeJS
- MSSQL or MYSQL or PostgreSQL server
## Configuration
In ```<Code Root>/SqGoods/appsettings.json``` update the following attributes

- **DbType** - ```MsSql``` or ```MySql``` or ```PgSql```
- **ConnectionString** - proper connection string to a selected server
## Running
1. go to  ```<Code Root>/SqGoods```
2. run ```dotnet run```
3. Open ```http://localhost:55242``` in a browser  
