#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DeveWeb3Cli/DeveWeb3Cli.csproj", "DeveWeb3Cli/"]
RUN dotnet restore "DeveWeb3Cli/DeveWeb3Cli.csproj"
COPY . .
WORKDIR "/src/DeveWeb3Cli"
RUN dotnet build "DeveWeb3Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeveWeb3Cli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeveWeb3Cli.dll"]