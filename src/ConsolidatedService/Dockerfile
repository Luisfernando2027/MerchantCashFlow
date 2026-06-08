FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy solution and project files to leverage caching and allow restore
COPY MerchantCashFlow.sln ./
COPY src/Shared/Shared.csproj src/Shared/
COPY src/ConsolidatedService/ConsolidatedService.csproj src/ConsolidatedService/

RUN dotnet restore MerchantCashFlow.sln

# copy remaining sources
COPY . ./

WORKDIR /src/src/ConsolidatedService
RUN dotnet publish ConsolidatedService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ConsolidatedService.dll"]
