#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Roomify.OidcServer/Roomify.OidcServer.csproj", "Roomify.OidcServer/"]
RUN dotnet restore "Roomify.OidcServer/Roomify.OidcServer.csproj"
COPY . .
WORKDIR "/src/Roomify.OidcServer"
RUN dotnet build "Roomify.OidcServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Roomify.OidcServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Roomify.OidcServer.dll"]