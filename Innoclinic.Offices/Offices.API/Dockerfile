#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Offices.API/Offices.API.csproj", "Offices.API/"]
COPY ["Offices.Application/Offices.Application.csproj", "Offices.Application/"]
COPY ["Offices.Application.Contracts/Offices.Application.Contracts.csproj", "Offices.Application.Contracts/"]
COPY ["Offices.Shared/Offices.Shared.csproj", "Offices.Shared/"]
COPY ["Offices.Contracts/Offices.Contracts.csproj", "Offices.Contracts/"]
COPY ["Offices.Domain/Offices.Domain.csproj", "Offices.Domain/"]
COPY ["Offices.Infrastructure/Offices.Infrastructure.csproj", "Offices.Infrastructure/"]
COPY ["Offices.Presentation/Offices.Presentation.csproj", "Offices.Presentation/"]
COPY ["Offices.API/Innoclinic.pfx", "Offices.API/"]
RUN dotnet restore "./Offices.API/Offices.API.csproj"
COPY . .
WORKDIR "/src/Offices.API"
RUN dotnet build "./Offices.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Offices.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Offices.API.dll"]