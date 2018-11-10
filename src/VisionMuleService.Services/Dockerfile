FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 59039
EXPOSE 44377

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["VisionMuleService.Services/VisionMuleService.Services.csproj", "VisionMuleService.Services/"]
RUN dotnet restore "VisionMuleService.Services/VisionMuleService.Services.csproj"
COPY . .
WORKDIR "/src/VisionMuleService.Services"
RUN dotnet build "VisionMuleService.Services.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "VisionMuleService.Services.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "VisionMuleService.Services.dll"]