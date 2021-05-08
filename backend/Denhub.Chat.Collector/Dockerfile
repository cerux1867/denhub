FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Denhub.Chat.Collector/Denhub.Chat.Collector.csproj", "Denhub.Chat.Collector/"]
RUN dotnet restore "Denhub.Chat.Collector/Denhub.Chat.Collector.csproj"
COPY . .
WORKDIR "/src/Denhub.Chat.Collector"
RUN dotnet build "Denhub.Chat.Collector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Denhub.Chat.Collector.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Denhub.Chat.Collector.dll"]
