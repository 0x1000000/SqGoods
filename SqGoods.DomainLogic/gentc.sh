#!/bin/bash

lib=~/.nuget/packages/sqexpress/$(ls ~/.nuget/packages/sqexpress -r|head -n 1)/tools/codegen/SqExpress.CodeGenUtil.dll

dotnet $lib gentables mssql "Data Source=(local);Initial Catalog=SqGoods;Integrated Security=True" --table-class-prefix "Tbl" -o "./Tables" -n "SqGoods.DomainLogic.Tables"