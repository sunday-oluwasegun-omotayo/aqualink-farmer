FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["AquaLink.Farmer.API/AquaLink.Farmer.API.csproj", "AquaLink.Farmer.API/"]
COPY ["AquaLink.Farmer.Application/AquaLink.Farmer.Application.csproj", "AquaLink.Farmer.Application/"]
COPY ["AquaLink.Farmer.Domain/AquaLink.Farmer.Domain.csproj", "AquaLink.Farmer.Domain/"]
COPY ["AquaLink.Farmer.Infrastructure/AquaLink.Farmer.Infrastructure.csproj", "AquaLink.Farmer.Infrastructure/"]
COPY ["AquaLink.Cooperative.Application/AquaLink.Cooperative.Application.csproj", "AquaLink.Cooperative.Application/"]
COPY ["AquaLink.Cooperative.Domain/AquaLink.Cooperative.Domain.csproj", "AquaLink.Cooperative.Domain/"]
COPY ["AquaLink.Cooperative.Infrastructure/AquaLink.Cooperative.Infrastructure.csproj", "AquaLink.Cooperative.Infrastructure/"]
COPY ["AquaLink.Prices.Application/AquaLink.Prices.Application.csproj", "AquaLink.Prices.Application/"]
COPY ["AquaLink.Prices.Domain/AquaLink.Prices.Domain.csproj", "AquaLink.Prices.Domain/"]
COPY ["AquaLink.Prices.Infrastructure/AquaLink.Prices.Infrastructure.csproj", "AquaLink.Prices.Infrastructure/"]

RUN dotnet restore "AquaLink.Farmer.API/AquaLink.Farmer.API.csproj"

COPY . .
WORKDIR "/src/AquaLink.Farmer.API"
RUN dotnet build "AquaLink.Farmer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AquaLink.Farmer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AquaLink.Farmer.API.dll"]