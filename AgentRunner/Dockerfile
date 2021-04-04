#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["AlgoRunner/AlgoRunner.csproj", "AlgoRunner/"]
RUN dotnet restore "AlgoRunner/AlgoRunner.csproj"
COPY . .
WORKDIR "/src/AlgoRunner"
RUN dotnet build "AlgoRunner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AlgoRunner.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlgoRunner.dll"]