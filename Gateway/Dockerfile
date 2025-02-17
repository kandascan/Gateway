# Etap bazowy (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etap build (kompilacja aplikacji)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Skopiowanie pliku .csproj i przywrócenie zależności
COPY ["Gateway.csproj", "."]
RUN dotnet restore "./Gateway.csproj"

# Skopiowanie reszty kodu i budowanie aplikacji
COPY . .
RUN dotnet build "./Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etap publikacji aplikacji
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etap finalny (uruchamianie aplikacji)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.dll"]