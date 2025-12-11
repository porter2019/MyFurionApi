#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# 安装 fontconfig
RUN apt-get update && apt-get install -y fontconfig && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 8080
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["MyFurionApi.Web.Entry/MyFurionApi.Web.Entry.csproj", "MyFurionApi.Web.Entry/"]
COPY ["MyFurionApi.Web.Core/MyFurionApi.Web.Core.csproj", "MyFurionApi.Web.Core/"]
COPY ["MyFurionApi.Core/MyFurionApi.Core.csproj", "MyFurionApi.Core/"]
COPY ["MyFurionApi.Application/MyFurionApi.Application.csproj", "MyFurionApi.Application/"]
RUN dotnet restore "MyFurionApi.Web.Entry/MyFurionApi.Web.Entry.csproj"
COPY . .
WORKDIR "/src/MyFurionApi.Web.Entry"
RUN dotnet build "MyFurionApi.Web.Entry.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "MyFurionApi.Web.Entry.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

# 复制项目内字体到容器
RUN mkdir -p /usr/share/fonts/truetype/custom
COPY CYERPApi.Web.Entry/wwwroot/fonts/*.ttf /usr/share/fonts/truetype/custom/
# 更新字体缓存
RUN fc-cache -fv

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyFurionApi.Web.Entry.dll"]