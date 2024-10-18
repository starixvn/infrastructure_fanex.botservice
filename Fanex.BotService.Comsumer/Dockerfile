ARG CI_REGISTRY_IMAGE

FROM $CI_REGISTRY_IMAGE:builder as deps

RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

ENV NUGET_PACKAGES=/src/packages

COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Fanex.BotService.Comsumer/Fanex.BotService.Comsumer.csproj", "Fanex.BotService.Comsumer/"]
COPY ["Fanex.BotService.Service.Core/Fanex.BotService.Service.Core.csproj", "Fanex.BotService.Service.Core/"]
COPY ["Fanex.BotService.Shared/Fanex.BotService.Shared.csproj", "Fanex.BotService.Shared/"]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

RUN dotnet restore -v n -r linux-x64 "./Fanex.BotService.Comsumer/./Fanex.BotService.Comsumer.csproj"

COPY . .

WORKDIR "/src/Fanex.BotService.Comsumer"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.BotService.Comsumer.dll"]