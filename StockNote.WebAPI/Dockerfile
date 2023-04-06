#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["StockNote.DataAccess/StockNote.DataAccess.csproj", "StockNote.DataAccess/"]
COPY ["StockNote.WebAPI/StockNote.WebAPI.csproj", "StockNote.WebAPI/"]
RUN dotnet restore "StockNote.WebAPI/StockNote.WebAPI.csproj"
COPY . .
WORKDIR "/src/StockNote.WebAPI"
RUN dotnet build "StockNote.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StockNote.WebAPI.csproj" -c Release -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockNote.WebAPI.dll"]