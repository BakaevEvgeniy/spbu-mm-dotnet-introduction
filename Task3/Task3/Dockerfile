FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

EXPOSE 80

WORKDIR /src
COPY Task3.csproj Task3.csproj
RUN dotnet restore Task3.csproj
COPY . .
WORKDIR /src
RUN dotnet build Task3.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Task3.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final

ENV TommorowApiKey=SOME_TOMMOROW_API_KEY
ENV StormGlassApiKey=SOME_STORMGLASS_API_KEY
ENV ASPNETCORE_ENVIRONMENT=Development

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Task3.dll"]