FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy solution and project files first to leverage layer caching and
# ensure referenced projects (Shared) are available for restore
COPY MerchantCashFlow.sln ./
COPY src/Shared/Shared.csproj src/Shared/
COPY src/LaunchesService/LaunchesService.csproj src/LaunchesService/

RUN dotnet restore MerchantCashFlow.sln

# copy the rest of the source
COPY . ./

WORKDIR /src/src/LaunchesService
RUN dotnet publish LaunchesService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "LaunchesService.dll"]
