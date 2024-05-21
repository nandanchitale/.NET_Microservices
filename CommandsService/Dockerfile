FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5122

ENV ASPNETCORE_URLS=http://+:5122

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["CommandsService/CommandsService.csproj", "CommandsService/"]
RUN dotnet restore "CommandsService/CommandsService.csproj"
COPY . .
WORKDIR "/src/CommandsService"
RUN dotnet build "CommandsService.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "CommandsService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommandsService.dll"]
