FROM registry.nexdev.net:5050/docker/dotnet/core/sdk:5.0.102-ca-patch-buster-slim AS builder

WORKDIR /src

COPY ["Fanex.NCB.Shared", "Fanex.NCB.Shared"]
COPY ["Fanex.NCB.BotService.Core", "Fanex.NCB.BotService.Core"]
COPY ["Fanex.NCB.BotService.EndPoint.csproj", "./"]
COPY ["nuget.config", ""]

WORKDIR "/Fanex.NCB.BotService.EndPoint"
RUN dotnet restore -v n

COPY ./Fanex.NCB.BotService.EndPoint .

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM registry.nexdev.net:5050/docker/dotnet/core/aspnet:5.0-buster-slim AS base

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "Fanex.NCB.BotService.EndPoint.dll"]
