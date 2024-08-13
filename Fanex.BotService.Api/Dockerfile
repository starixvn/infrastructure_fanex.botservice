FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Fanex.BotService.Api/Fanex.BotService.Api.csproj", "Fanex.BotService.Api/"]
COPY ["Fanex.BotService.Service.Core/Fanex.BotService.Service.Core.csproj", "Fanex.BotService.Service.Core/"]
COPY ["Fanex.BotService.Shared/Fanex.BotService.Shared.csproj", "Fanex.BotService.Shared/"]

RUN dotnet restore "./Fanex.BotService.Api/./Fanex.BotService.Api.csproj"
COPY . .

WORKDIR "/src/Fanex.BotService.Api"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.BotService.Api.dll"]