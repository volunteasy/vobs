FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build-env
WORKDIR /app

# Copy everything
COPY . .

# Restore as distinct layers
RUN dotnet restore


# Build and publish a release. As Web is our startup project, we're publish it only, so it builds from this perspective
RUN dotnet publish -c Release -o out src/Web


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Web.dll"]