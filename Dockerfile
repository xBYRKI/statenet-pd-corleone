# Verwende das .NET SDK-Basisimage für den Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Kopiere die csproj-Datei und führe dotnet restore aus
COPY statenet-lspd.csproj ./  
RUN dotnet restore "statenet-lspd.csproj"

# Kopiere den Rest des Projekts und baue das Projekt
COPY . .  
WORKDIR /app/statenet_lspd  
RUN dotnet build "statenet-lspd.csproj" -c Release -o /app/build

# Veröffentliche die Anwendung
FROM build AS publish
RUN dotnet publish "statenet-lspd.csproj" -c Release -o /app/publish

# Erstelle das endgültige Image mit ASP.NET Core Runtime-Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Kopiere die veröffentlichten Dateien aus dem vorherigen Schritt
COPY --from=publish /app/publish .

# Kopiere das wwwroot-Verzeichnis, wenn es im Stammverzeichnis vorhanden ist


# Setze den Einstiegspunkt
ENTRYPOINT ["dotnet", "statenet-lspd.dll"]
