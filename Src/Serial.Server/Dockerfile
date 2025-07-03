FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim-arm64v8 AS base
#FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim-arm32v7 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim-arm64v8 AS build
RUN apt-get -y update \
    && curl -sL https://deb.nodesource.com/setup_24.x | bash - \ 
    && apt-get install -y nodejs \
    && apt-get clean \
    && echo 'node verions:' $(node -v) \
    && echo 'npm version:' $(npm -v) \
    && echo 'dotnet version:' $(dotnet --version)
WORKDIR /Src
COPY Directory.Build.props ./
COPY ["Framework/Arduino/Arduino.csproj", "Framework/Arduino/"]
COPY ["Framework/Dependency/Dependency.csproj", "Framework/Dependency/"]
COPY ["Framework/Directory.Build.props", "Framework/"]
COPY ["Framework/Drawing/Drawing.csproj", "Framework/Drawing/"]
COPY ["Framework/Localization/Localization.csproj", "Framework/Localization/"]
COPY ["Framework/Logic.Abstraction/Logic.Abstraction.csproj", "Framework/Logic.Abstraction/"]
COPY ["Framework/NLogTools/NLogTools.csproj", "Framework/NLogTools/"]
COPY ["Framework/Parser/Parser.csproj", "Framework/Parser/"]
COPY ["Framework/Pattern/Pattern.csproj", "Framework/Pattern/"]
COPY ["Framework/Tools/Tools.csproj", "Framework/Tools/"]
COPY ["Framework/WebAPI.Host/WebAPI.Host.csproj", "Framework/WebAPI.Host/"]
COPY ["Framework/WebAPI/WebAPI.csproj", "Framework/WebAPI/"]
COPY ["Framework/WinAPI/WinAPI.csproj", "Framework/WinAPI/"]
COPY ["GCode.Draw/GCode.Draw.csproj", "GCode.Draw/"]
COPY ["GCode.Generate/GCode.Generate.csproj", "GCode.Generate/"]
COPY ["GCode.Machine/GCode.Machine.csproj", "GCode.Machine/"]
COPY ["GCode.Serial/GCode.Serial.csproj", "GCode.Serial/"]
COPY ["GCode.Tools/GCode.Tools.csproj", "GCode.Tools/"]
COPY ["Logic.Abstraction.DTO/Logic.Abstraction.DTO.csproj", "Logic.Abstraction.DTO/"]
COPY ["Serial.Server/Serial.Server.csproj", "Serial.Server/"]
COPY ["Serial.Shared/Serial.Shared.csproj", "Serial.Shared/"]
COPY ["Serial.WebAPI/Serial.WebAPI.csproj", "Serial.WebAPI/"]
RUN dotnet restore "/Src/Serial.Server/Serial.Server.csproj"
COPY . .
#WORKDIR "/src/Serial.Server"
#RUN dotnet build "Serial.Server.csproj" -c Release -o /app/build -r linux-arm

FROM build AS publish
WORKDIR "/Src/Serial.Server"
RUN dotnet publish "Serial.Server.csproj" -c Release -o /app/publish --no-self-contained -r linux-arm64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CNCLib.Serial.Server.dll"]
