EF Core - 3.1 

1. Install EF
Nuget-packages:
- install-package Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Relational
- Microsoft.EntityFrameworkCore

2. Migration
Nuget-package for Migration:
- Powershell: Microsoft.EntityFrameworkCore.Tools
- dotnet CLI: Microsoft.EntityFrameworkCore.Tools.Dotnet

Powershell commands:
- get-help entityframework
- add-migration name
- script-migration
- update-database -verbose

3. Reverse engineering
Nuget-packages:
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools

Powershell command:
scaffold-dbcontext -provider Microsoft.EntityFrameworkCore.SqlServer
				   -connection "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = SamuraiAppData"	