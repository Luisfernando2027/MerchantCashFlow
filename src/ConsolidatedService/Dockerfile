FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/ConsolidatedService/ConsolidatedService.csproj src/ConsolidatedService/
COPY src/Shared/Shared.csproj src/Shared/
RUN dotnet restore src/ConsolidatedService/ConsolidatedService.csproj
COPY . .
RUN dotnet publish src/ConsolidatedService/ConsolidatedService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ConsolidatedService.dll"]
