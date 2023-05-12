FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Volunteasy.Api/Volunteasy.Api.csproj", "src/Volunteasy.Api/"]
RUN dotnet restore "src/Volunteasy.Api/Volunteasy.Api.csproj"
COPY . .
WORKDIR "/src/src/Volunteasy.Api"
RUN dotnet build "Volunteasy.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Volunteasy.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Volunteasy.Api.dll"]
