# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY . .
WORKDIR /app/src/CarAssistant

RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "CarAssistant.dll"]