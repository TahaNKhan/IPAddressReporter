#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["IPAddressReporter/IPAddressReporter.csproj", "IPAddressReporter/"]
RUN dotnet restore "IPAddressReporter/IPAddressReporter.csproj"
COPY . .
WORKDIR "/src/IPAddressReporter"
RUN dotnet build "IPAddressReporter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IPAddressReporter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IPAddressReporter.dll"]