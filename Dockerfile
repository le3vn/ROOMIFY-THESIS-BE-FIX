# Use the .NET 8.0 SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Roomify.WebApi/Roomify.WebApi.csproj", "Roomify.WebApi/"]
RUN dotnet restore "Roomify.WebApi/Roomify.WebApi.csproj"
COPY . .
WORKDIR "/src/Roomify.WebApi"
RUN dotnet build "Roomify.WebApi.csproj" -c Release -o /app/build

# Publish the application in release mode
FROM build AS publish
RUN dotnet publish "Roomify.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the .NET 8.0 ASP.NET runtime for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Roomify.WebApi.dll"]
