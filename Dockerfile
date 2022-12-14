FROM mcr.microsoft.com/dotnet/runtime:7.0-bullseye-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim AS build
WORKDIR /src
COPY ["Koala.Messaging.Publisher.Service.csproj", "./"]
RUN dotnet restore "Koala.Messaging.Publisher.Service.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Koala.Messaging.Publisher.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Koala.Messaging.Publisher.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Koala.MessagePublisherService.dll"]