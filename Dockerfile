FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Koala.Messaging.Publisher.Service.csproj", "./"]
RUN dotnet restore "Koala.Messaging.Publisher.Service.csproj" -r linux-arm64
COPY . .
WORKDIR "/src"
RUN dotnet build "Koala.Messaging.Publisher.Service.csproj" -c Release -o /app/build -r linux-arm64

FROM build AS publish
RUN dotnet publish "Koala.Messaging.Publisher.Service.csproj" -c Release -o /app/publish -r linux-arm64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Koala.MessagePublisherService.dll"]
