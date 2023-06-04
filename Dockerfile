FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebApplicationDotNet6Kafka/WebApplicationDotNet6Kafka.csproj", "WebApplicationDotNet6Kafka/"]
RUN dotnet restore "WebApplicationDotNet6Kafka/WebApplicationDotNet6Kafka.csproj"
COPY . .
WORKDIR "/src/WebApplicationDotNet6Kafka"
RUN dotnet build "WebApplicationDotNet6Kafka.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApplicationDotNet6Kafka.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplicationDotNet6Kafka.dll"]