## Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy csproj and restore as distinct layers
# Use BuildKit cache mount for NuGet packages to speed up restores
COPY StargateAPI.csproj ./
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore

# Copy the rest of the source and build
# Use BuildKit cache mounts for NuGet packages and build cache to speed up subsequent builds
COPY . ./
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    --mount=type=cache,id=build,target=/root/.dotnet/build \
    dotnet publish -c Release -o /app/publish

## Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish ./

# Configure ASP.NET Core to listen on port 8080 inside the container
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "StargateAPI.dll"]


