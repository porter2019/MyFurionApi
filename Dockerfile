#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MyFurionApi.Web.Entry/MyFurionApi.Web.Entry.csproj", "MyFurionApi.Web.Entry/"]
COPY ["MyFurionApi.Web.Core/MyFurionApi.Web.Core.csproj", "MyFurionApi.Web.Core/"]
COPY ["MyFurionApi.Application2/MyFurionApi.Application2.csproj", "MyFurionApi.Application2/"]
COPY ["MyFurionApi.Core/MyFurionApi.Core.csproj", "MyFurionApi.Core/"]
COPY ["MyFurionApi.Application/MyFurionApi.Application.csproj", "MyFurionApi.Application/"]
RUN dotnet restore "MyFurionApi.Web.Entry/MyFurionApi.Web.Entry.csproj"
COPY . .
WORKDIR "/src/MyFurionApi.Web.Entry"
RUN dotnet build "MyFurionApi.Web.Entry.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyFurionApi.Web.Entry.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyFurionApi.Web.Entry.dll"]