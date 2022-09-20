FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Zefi.Telegram.Bot.TimeTracking/Zefi.Telegram.Bot.TimeTracking.csproj", "Zefi.Telegram.Bot.TimeTracking/"]
RUN dotnet restore "Zefi.Telegram.Bot.TimeTracking/Zefi.Telegram.Bot.TimeTracking.csproj"
COPY . .
WORKDIR "/src/Zefi.Telegram.Bot.TimeTracking"
RUN dotnet build "Zefi.Telegram.Bot.TimeTracking.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Zefi.Telegram.Bot.TimeTracking.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zefi.Telegram.Bot.TimeTracking.dll"]
