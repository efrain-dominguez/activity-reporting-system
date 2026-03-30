# Activity Reporting System - Backend Setup Script
# Clean Architecture with .NET 9

Write-Host "Creating Activity Reporting System - Backend..." -ForegroundColor Green

# Create solution
dotnet new sln -n ActivityReportingSystem

# Create Domain project (Core business logic)
dotnet new classlib -n ARS.Domain -f net9.0
dotnet sln add ARS.Domain/ARS.Domain.csproj

# Create Application project (Use cases and services)
dotnet new classlib -n ARS.Application -f net9.0
dotnet sln add ARS.Application/ARS.Application.csproj

# Create Infrastructure project (Data access, external services)
dotnet new classlib -n ARS.Infrastructure -f net9.0
dotnet sln add ARS.Infrastructure/ARS.Infrastructure.csproj

# Create API project (Web API)
dotnet new webapi -n ARS.API -f net9.0
dotnet sln add ARS.API/ARS.API.csproj

Write-Host "Adding project references..." -ForegroundColor Yellow

# Add project references (following Clean Architecture)
# Domain has no dependencies
# Application depends on Domain
dotnet add ARS.Application/ARS.Application.csproj reference ARS.Domain/ARS.Domain.csproj

# Infrastructure depends on Domain and Application
dotnet add ARS.Infrastructure/ARS.Infrastructure.csproj reference ARS.Domain/ARS.Domain.csproj
dotnet add ARS.Infrastructure/ARS.Infrastructure.csproj reference ARS.Application/ARS.Application.csproj

# API depends on Application and Infrastructure
dotnet add ARS.API/ARS.API.csproj reference ARS.Application/ARS.Application.csproj
dotnet add ARS.API/ARS.API.csproj reference ARS.Infrastructure/ARS.Infrastructure.csproj

Write-Host "Installing NuGet packages..." -ForegroundColor Yellow

# Domain packages
dotnet add ARS.Domain/ARS.Domain.csproj package MongoDB.Bson

# Application packages
dotnet add ARS.Application/ARS.Application.csproj package AutoMapper
dotnet add ARS.Application/ARS.Application.csproj package FluentValidation

# Infrastructure packages
dotnet add ARS.Infrastructure/ARS.Infrastructure.csproj package MongoDB.Driver
dotnet add ARS.Infrastructure/ARS.Infrastructure.csproj package Microsoft.Identity.Web
dotnet add ARS.Infrastructure/ARS.Infrastructure.csproj package Azure.Storage.Blobs
dotnet add ARS.Infrastructure/ARS.Infrastructure.csproj package Serilog.AspNetCore

# API packages
dotnet add ARS.API/ARS.API.csproj package Microsoft.Identity.Web
dotnet add ARS.API/ARS.API.csproj package Swashbuckle.AspNetCore
dotnet add ARS.API/ARS.API.csproj package Serilog.AspNetCore

Write-Host "Cleaning up default files..." -ForegroundColor Yellow

# Remove default Class1.cs files
Remove-Item ARS.Domain/Class1.cs -ErrorAction SilentlyContinue
Remove-Item ARS.Application/Class1.cs -ErrorAction SilentlyContinue
Remove-Item ARS.Infrastructure/Class1.cs -ErrorAction SilentlyContinue

# Remove default WeatherForecast files from API
Remove-Item ARS.API/WeatherForecast.cs -ErrorAction SilentlyContinue
Remove-Item ARS.API/Controllers/WeatherForecastController.cs -ErrorAction SilentlyContinue

Write-Host "Creating folder structure..." -ForegroundColor Yellow

# Domain folders
New-Item -ItemType Directory -Path ARS.Domain/Entities -Force
New-Item -ItemType Directory -Path ARS.Domain/Enums -Force
New-Item -ItemType Directory -Path ARS.Domain/Exceptions -Force

# Application folders
New-Item -ItemType Directory -Path ARS.Application/DTOs -Force
New-Item -ItemType Directory -Path ARS.Application/Services -Force
New-Item -ItemType Directory -Path ARS.Application/Interfaces -Force
New-Item -ItemType Directory -Path ARS.Application/Validators -Force

# Infrastructure folders
New-Item -ItemType Directory -Path ARS.Infrastructure/Data -Force
New-Item -ItemType Directory -Path ARS.Infrastructure/Repositories -Force
New-Item -ItemType Directory -Path ARS.Infrastructure/Auth -Force
New-Item -ItemType Directory -Path ARS.Infrastructure/Storage -Force

# API folders
New-Item -ItemType Directory -Path ARS.API/Middleware -Force

Write-Host "Backend structure created successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Open ActivityReportingSystem.sln in Visual Studio"
Write-Host "2. Review the project structure"
Write-Host "3. We'll start creating domain models"