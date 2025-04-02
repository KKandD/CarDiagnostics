# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the solution file
COPY ["CarDiagnostics.sln", "./"]

# Copy all projects and restore
COPY ["CarDiagnostics.Application/CarDiagnostics.Application.csproj", "CarDiagnostics.Application/"]
COPY ["CarDiagnostics.Domain/CarDiagnostics.Domain.csproj", "CarDiagnostics.Domain/"]
COPY ["CarDiagnostics.Infrastructure/CarDiagnostics.Infrastructure.csproj", "CarDiagnostics.Infrastructure/"]
COPY ["CarDiagnostics/CarDiagnostics.csproj", "CarDiagnostics/"]

RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build all projects
WORKDIR "/src/CarDiagnostics"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarDiagnostics.dll"]
