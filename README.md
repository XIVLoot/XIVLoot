# FFXIV-RaidLootAPI
This is the backend of a loot management project so we can easily keep track of the raid loot in FFXIV. Hopefully I don't get so busy that I just stop working on it.
## Prepare your environment

You need

* Your favorite IDE (I recommend [Jetbrains Rider EAP](https://www.jetbrains.com/lp/toolbox/) or [Visual Studio](https://visualstudio.microsoft.com/vs/))
* [SQL Express](https://www.microsoft.com/en-us/download/details.aspx?id=101064)
* [SQL Server Management Studio](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)
* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Update your database

Using EF Core Tools, Update your database Use the UI if you are using Riders. If you are using Visual Studio, use the package manager terminal and enter the command `Update-Database`. If you are using anything else, open a terminal and use the command `dotnet ef update-database`
