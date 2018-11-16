FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["VisionMuleService.Controllers/VisionMuleService.Controllers.csproj", "VisionMuleService.Controllers/"]
COPY ["VisionMuleService.Services/VisionMuleService.Services.csproj", "VisionMuleService.Services/"]
RUN dotnet restore "VisionMuleService.Controllers/VisionMuleService.Controllers.csproj"
COPY . .
WORKDIR "/src/VisionMuleService.Controllers"
RUN dotnet build "VisionMuleService.Controllers.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "VisionMuleService.Controllers.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "VisionMuleService.Controllers.dll"]