# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["ARS.API/ARS.API.csproj", "ARS.API/"]
COPY ["ARS.Application/ARS.Application.csproj", "ARS.Application/"]
COPY ["ARS.Infrastructure/ARS.Infrastructure.csproj", "ARS.Infrastructure/"]
COPY ["ARS.Domain/ARS.Domain.csproj", "ARS.Domain/"]

# Restore dependencies
RUN dotnet restore "ARS.API/ARS.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/ARS.API"
RUN dotnet build "ARS.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "ARS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ARS.API.dll"]