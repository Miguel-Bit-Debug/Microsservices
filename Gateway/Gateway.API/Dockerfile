#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Gateway.API/Gateway.API.csproj", "Gateway.API/"]
COPY ["Gateway.CrossCutting/Gateway.CrossCutting.csproj", "Gateway.CrossCutting/"]
COPY ["Gateway.Domain/Gateway.Domain.csproj", "Gateway.Domain/"]
COPY . .

RUN dotnet restore "Gateway.API/Gateway.API.csproj"
RUN dotnet restore "Gateway.CrossCutting/Gateway.CrossCutting.csproj"
RUN dotnet restore "Gateway.Domain/Gateway.Domain.csproj"

WORKDIR "/src/Gateway.API"
RUN dotnet build "Gateway.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gateway.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.API.dll"]
