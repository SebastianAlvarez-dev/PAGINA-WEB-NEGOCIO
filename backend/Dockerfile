FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json Directory.Build.props Directory.Packages.props JewelryStore.slnx ./
COPY .config ./.config
COPY backend ./backend

RUN dotnet restore backend/src/JewelryStore.Api/JewelryStore.Api.csproj
RUN dotnet publish backend/src/JewelryStore.Api/JewelryStore.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_EnableDiagnostics=0
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "JewelryStore.Api.dll"]
