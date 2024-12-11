FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG CERT_PASS
ENV CERT_PASS $CERT_PASS

RUN mkdir https
RUN dotnet dev-certs https -ep ./https/aspnetapp.pfx -p ${CERT_PASS}
RUN dotnet dev-certs https --trust

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BotManager.Main/BotManager.Main.csproj", "BotManager.Main/"]
COPY ["BotManager.Db/BotManager.Db.csproj", "BotManager.Db/"]
COPY ["BotManager.Services/BotManager.Services.csproj", "BotManager.Services/"]
COPY ["BotManager.Bot/BotManager.Bot.csproj", "BotManager.Bot/"]
COPY ["BotManager.Interfaces/BotManager.Interfaces.csproj", "BotManager.Interfaces/"]
COPY ["BotManager.DI/BotManager.DI.csproj", "BotManager.DI/"]
RUN dotnet restore "BotManager.Main/BotManager.Main.csproj"
COPY . .
WORKDIR "/src/BotManager.Main"
RUN dotnet build "BotManager.Main.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BotManager.Main.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --chmod=0755 --from=build /https/* /https/
COPY --from=publish /app/publish .

RUN mkdir /app/dbdata
RUN mkdir /app/image

ENTRYPOINT ["dotnet", "BotManager.Main.dll"]
