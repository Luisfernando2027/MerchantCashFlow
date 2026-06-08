FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# copy only csproj and restore first for better caching
COPY src/LaunchesService/LaunchesService.csproj src/LaunchesService/
COPY src/Shared/Shared.csproj src/Shared/
RUN dotnet restore src/LaunchesService/LaunchesService.csproj
COPY . .
RUN dotnet publish src/LaunchesService/LaunchesService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "LaunchesService.dll"]
