FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProjectMetadataPlatform.Api/ProjectMetadataPlatform.Api.csproj", "ProjectMetadataPlatform.Api/"]
COPY ["ProjectMetadataPlatform.Application/ProjectMetadataPlatform.Application.csproj", "ProjectMetadataPlatform.Application/"]
COPY ["ProjectMetadataPlatform.Domain/ProjectMetadataPlatform.Domain.csproj", "ProjectMetadataPlatform.Domain/"]
COPY ["ProjectMetadataPlatform.Infrastructure/ProjectMetadataPlatform.Infrastructure.csproj", "ProjectMetadataPlatform.Infrastructure/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/ProjectMetadataPlatform.Api"
RUN dotnet build "ProjectMetadataPlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjectMetadataPlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectMetadataPlatform.Api.dll"]
