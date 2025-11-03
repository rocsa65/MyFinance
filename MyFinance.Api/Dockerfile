# multi-stage build for .NET 9 ASP.NET app (optimized for cache)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj(s) and restore separately to leverage cache
COPY ["MyFinance.Api/MyFinance.Api.csproj", "MyFinance.Api/"]
RUN dotnet restore "MyFinance.Api/MyFinance.Api.csproj"

# Copy remaining source
COPY . .

# Publish
RUN dotnet publish "MyFinance.Api/MyFinance.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN mkdir -p /data
ENV ConnectionStrings__DefaultConnection="Data Source=/data/finance.db"
ENV ASPNETCORE_URLS="http://+:80"

EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MyFinance.Api.dll"]