FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app

ENV ASPNETCORE_HTTP_PORTS=8081
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8081

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TrakfinAPI.csproj", "."]
RUN dotnet restore "./TrakfinAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./TrakfinAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TrakfinAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrakfinAPI.dll"]