FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CarGuesser.Web/CarGuesser.Web.csproj", "CarGuesser.Web/"]
RUN dotnet restore "CarGuesser.Web/CarGuesser.Web.csproj"
COPY . .
WORKDIR "/src/CarGuesser.Web"
RUN dotnet build "CarGuesser.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CarGuesser.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarGuesser.Web.dll"]
