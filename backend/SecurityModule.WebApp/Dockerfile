# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SecurityModule.WebApp/SecurityModule.WebApp.csproj", "SecurityModule.WebApp/"]
COPY ["SecurityModule.BLL/SecurityModule.BLL.csproj", "SecurityModule.BLL/"]
COPY ["VendingManagement.Shared/VendingManagement.Shared.csproj", "VendingManagement.Shared/"]
RUN dotnet restore "./SecurityModule.WebApp/SecurityModule.WebApp.csproj"
COPY . .
WORKDIR "/src/SecurityModule.WebApp"
RUN dotnet build "./SecurityModule.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SecurityModule.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecurityModule.WebApp.dll"]