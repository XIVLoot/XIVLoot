# XIVLoot
![.net build workflow](https://github.com/RickyChittaphone/FFXIV-RaidLootAPI/actions/workflows/dotnet.yml/badge.svg) &nbsp; ![Docker Image Workflow](https://github.com/XIVLoot/XIVLoot/actions/workflows/build-docker-dev-api.yml/badge.svg)<br>
This is a FFXIV raid loot management project. This includes a front-end and a back-end. Front-end is in Angular and the Backend is in .NET using EntityFramework.
## Prepare your environment

You need

* Your favorite IDE (I recommend [Jetbrains Rider EAP](https://www.jetbrains.com/lp/toolbox/) or [Visual Studio](https://visualstudio.microsoft.com/vs/))
* [SQL Express](https://www.microsoft.com/en-us/download/details.aspx?id=101064)
* [SQL Server Management Studio](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)
* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Docker Desktop](https://docs.docker.com/desktop/install/windows-install/)

## Docker


### Create your SSL certificate

```shell
dotnet dev-certs https --clean
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p XIVLoot
dotnet dev-certs https --trust
```

### Build the image

From the root of the project (where the .sln is)
```shell
docker build -f FFXIV-RaidLootAPI/Dockerfile -t ffxiv-raidlootapi .
```

### Put the images up

Go into the docker-db folder in the project
```shell
docker compose up -d
```

and if you want to put the containers down

```shell
docker compose down
```

