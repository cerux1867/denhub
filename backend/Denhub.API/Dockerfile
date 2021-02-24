FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Denhub.API/Denhub.API.csproj", "Denhub.API/"]
RUN dotnet restore "Denhub.API/Denhub.API.csproj"
COPY . .
WORKDIR "/src/Denhub.API"
RUN dotnet build "Denhub.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Denhub.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Denhub.API.dll"]
