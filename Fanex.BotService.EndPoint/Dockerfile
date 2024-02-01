FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Fanex.NCB.BotService.EndPoint/Fanex.NCB.BotService.EndPoint.csproj", "Fanex.NCB.BotService.EndPoint/"]
COPY ["Fanex.NCB.BotService.Core/Fanex.NCB.BotService.Core.csproj", "Fanex.NCB.BotService.Core/"]
COPY ["Fanex.NCB.Shared/Fanex.NCB.Shared.csproj", "Fanex.NCB.Shared/"]
COPY ["Fanex.NCB.Resources/Fanex.NCB.Resources.csproj", "Fanex.NCB.Resources/"]
RUN dotnet restore "Fanex.NCB.BotService.EndPoint/Fanex.NCB.BotService.EndPoint.csproj"

COPY . .

WORKDIR "/src/Fanex.NCB.BotService.EndPoint"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.NCB.BotService.EndPoint.dll"]