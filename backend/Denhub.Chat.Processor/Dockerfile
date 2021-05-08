FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Denhub.Chat.Processor/Denhub.Chat.Processor.csproj", "Denhub.Chat.Processor/"]
RUN dotnet restore "Denhub.Chat.Processor/Denhub.Chat.Processor.csproj"
COPY . .
WORKDIR "/src/Denhub.Chat.Processor"
RUN dotnet build "Denhub.Chat.Processor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Denhub.Chat.Processor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Denhub.Chat.Processor.dll"]
