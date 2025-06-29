# FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim-arm64v8 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
RUN apt-get -y update \
    && curl -sL https://deb.nodesource.com/setup_24.x | bash - \ 
    && apt-get install -y nodejs \
    && apt-get clean \
    && echo 'node verions:' $(node -v) \
    && echo 'npm version:' $(npm -v) \
    && echo 'dotnet version:' $(dotnet --version)
WORKDIR /Src
COPY ["Directory.Build.props", "."]
COPY ["Framework/Directory.Build.props", "Framework/"]
COPY ["Server/Server.csproj", "Server/"]
COPY ["Framework/NLogTools/NLogTools.csproj", "Framework/NLogTools/"]
COPY ["Framework/Repository/Repository.csproj", "Framework/Repository/"]
COPY ["Framework/CsvImport/CsvImport.csproj", "Framework/CsvImport/"]
COPY ["Framework/Localization/Localization.csproj", "Framework/Localization/"]
COPY ["Framework/Repository.Abstraction/Repository.Abstraction.csproj", "Framework/Repository.Abstraction/"]
COPY ["Framework/Tools/Tools.csproj", "Framework/Tools/"]
COPY ["Framework/Dependency/Dependency.csproj", "Framework/Dependency/"]
COPY ["Framework/Parser/Parser.csproj", "Framework/Parser/"]
COPY ["Framework/Schedule/Schedule.csproj", "Framework/Schedule/"]
COPY ["Framework/WebAPI.Host/WebAPI.Host.csproj", "Framework/WebAPI.Host/"]
COPY ["Framework/WinAPI/WinAPI.csproj", "Framework/WinAPI/"]
COPY ["Framework/WebAPI/WebAPI.csproj", "Framework/WebAPI/"]
COPY ["Framework/Logic.Abstraction/Logic.Abstraction.csproj", "Framework/Logic.Abstraction/"]
COPY ["GCode.Generate/GCode.Generate.csproj", "GCode.Generate/"]
COPY ["Framework/Drawing/Drawing.csproj", "Framework/Drawing/"]
COPY ["Logic.Abstraction.DTO/Logic.Abstraction.DTO.csproj", "Logic.Abstraction.DTO/"]
COPY ["Logic.Client/Logic.Client.csproj", "Logic.Client/"]
COPY ["Framework/Pattern/Pattern.csproj", "Framework/Pattern/"]
COPY ["Service.Abstraction/Service.Abstraction.csproj", "Service.Abstraction/"]
COPY ["Framework/Service.Abstraction/Service.Abstraction.csproj", "Framework/Service.Abstraction/"]
COPY ["Logic.Abstraction/Logic.Abstraction.csproj", "Logic.Abstraction/"]
COPY ["GCode.Machine/GCode.Machine.csproj", "GCode.Machine/"]
COPY ["Framework/Arduino/Arduino.csproj", "Framework/Arduino/"]
COPY ["GCode.Tools/GCode.Tools.csproj", "GCode.Tools/"]
COPY ["Logic/Logic.csproj", "Logic/"]
COPY ["Framework/Logic/Logic.csproj", "Framework/Logic/"]
COPY ["Repository.Abstraction/Repository.Abstraction.csproj", "Repository.Abstraction/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Repository.SqLite/Repository.SqLite.csproj", "Repository.SqLite/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Service.Logic/Service.Logic.csproj", "Service.Logic/"]
COPY ["Framework/Service.Logic/Service.Logic.csproj", "Framework/Service.Logic/"]
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
COPY ["GCode.Draw/GCode.Draw.csproj", "GCode.Draw/"]
RUN dotnet restore "./Server/./Server.csproj"
COPY . .
#WORKDIR "/src/Server"
#RUN dotnet build "Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/Src/Server"
RUN dotnet publish "Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false -r linux-arm64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CNCLib.Server.dll"]
RUN mkdir /app/data
USER 0
RUN mkdir /var/log/CNCLib.Server
RUN chmod 0777 /var/log/CNCLib.Server
