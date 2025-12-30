FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BankingSystemAPI.API/BankingSystemAPI.API.csproj", "BankingSystemAPI.API/"]
COPY ["BankingSystemAPI.Core/BankingSystemAPI.Core.csproj", "BankingSystemAPI.Core/"]
COPY ["BankingSystemAPI.Data/BankingSystemAPI.Data.csproj", "BankingSystemAPI.Data/"]
COPY ["BankingSystemAPI.Services/BankingSystemAPI.Services.csproj", "BankingSystemAPI.Services/"]
RUN dotnet restore "BankingSystemAPI.API/BankingSystemAPI.API.csproj"
COPY . .
WORKDIR "/src/BankingSystemAPI.API"
RUN dotnet build "./BankingSystemAPI.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BankingSystemAPI.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankingSystemAPI.API.dll"]
