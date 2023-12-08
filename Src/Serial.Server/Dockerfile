#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim-arm32v7 AS base
# RUN apt-get update && apt-get install -y libgdiplus
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y build-essential nodejs
WORKDIR /src
COPY Directory.Build.props ./
COPY ["Framework/Directory.Build.props", "Framework/"]
COPY ["Serial.Server/Serial.Server.csproj", "Serial.Server/"]
COPY ["Framework/NLogTools/NLogTools.csproj", "Framework/NLogTools/"]
COPY ["Framework/Tools/Tools.csproj", "Framework/Tools/"]
COPY ["Framework/Dependency/Dependency.csproj", "Framework/Dependency/"]
COPY ["Framework/Localization/Localization.csproj", "Framework/Localization/"]
COPY ["Framework/Parser/Parser.csproj", "Framework/Parser/"]
COPY ["Framework/Arduino/Arduino.csproj", "Framework/Arduino/"]
COPY ["Framework/Pattern/Pattern.csproj", "Framework/Pattern/"]
COPY ["Framework/WinAPI/WinAPI.csproj", "Framework/WinAPI/"]
COPY ["Framework/WebAPI.Host/WebAPI.Host.csproj", "Framework/WebAPI.Host/"]
COPY ["Framework/WebAPI/WebAPI.csproj", "Framework/WebAPI/"]
COPY ["Framework/Logic.Abstraction/Logic.Abstraction.csproj", "Framework/Logic.Abstraction/"]
COPY ["Serial.Shared/Serial.Shared.csproj", "Serial.Shared/"]
COPY ["Serial.WebAPI/Serial.WebAPI.csproj", "Serial.WebAPI/"]
COPY ["GCode.Draw/GCode.Draw.csproj", "GCode.Draw/"]
COPY ["Framework/Drawing/Drawing.csproj", "Framework/Drawing/"]
COPY ["GCode.Generate/GCode.Generate.csproj", "GCode.Generate/"]
COPY ["Logic.Abstraction.DTO/Logic.Abstraction.DTO.csproj", "Logic.Abstraction.DTO/"]
COPY ["GCode.Serial/GCode.Serial.csproj", "GCode.Serial/"]
COPY ["GCode.Machine/GCode.Machine.csproj", "GCode.Machine/"]
COPY ["GCode.Tools/GCode.Tools.csproj", "GCode.Tools/"]
RUN dotnet restore "Serial.Server/Serial.Server.csproj"
COPY . .
#WORKDIR "/src/Serial.Server"
#RUN dotnet build "Serial.Server.csproj" -c Release -o /app/build -r linux-arm

FROM build AS publish
WORKDIR "/src/Serial.Server"
RUN dotnet publish "Serial.Server.csproj" -c Release -o /app/publish --no-self-contained -r linux-arm

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CNCLib.Serial.Server.dll"]