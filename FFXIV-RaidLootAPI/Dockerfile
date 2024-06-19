#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FFXIV-RaidLootAPI/FFXIV-RaidLootAPI.csproj", "FFXIV-RaidLootAPI/"]
RUN dotnet restore "./FFXIV-RaidLootAPI/FFXIV-RaidLootAPI.csproj"
COPY . .
WORKDIR "/src/FFXIV-RaidLootAPI"
RUN dotnet build "./FFXIV-RaidLootAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FFXIV-RaidLootAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FFXIV-RaidLootAPI.dll"]