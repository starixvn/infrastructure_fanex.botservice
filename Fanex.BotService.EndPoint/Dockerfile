FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Fanex.BotService.EndPoint/Fanex.BotService.EndPoint.csproj", "Fanex.BotService.EndPoint/"]
COPY ["Fanex.BotService.Core/Fanex.BotService.Core.csproj", "Fanex.BotService.Core/"]
COPY ["Fanex.BotService.Resources/Fanex.BotService.Resources.csproj", "Fanex.BotService.Resources/"]
COPY ["Fanex.BotService.Shared/Fanex.BotService.Shared.csproj", "Fanex.BotService.Shared/"]
RUN dotnet restore "./Fanex.BotService.EndPoint/./Fanex.BotService.EndPoint.csproj"
COPY . .
WORKDIR "/src/Fanex.BotService.EndPoint"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.BotService.EndPoint.dll"]