FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim-arm32v7 AS base
#FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim-arm32v7 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update && \
    apt-get install nodejs -y && \ 
    apt-get install npm -y
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
RUN dotnet publish "Serial.Server.csproj" -c Release -o /app/publish --no-self-contained -r linux-arm

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CNCLib.Serial.Server.dll"]
