FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR ..
COPY ["OrderService/OrderService.Api/OrderService.Api.csproj", "OrderService.Api/"]
COPY ["OrderService/OrderService.Application/OrderService.Application.csproj", "OrderService.Application/"]
COPY ["OrderService/OrderService.Domain/OrderService.Domain.csproj", "OrderService.Domain/"]
COPY ["OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "OrderService.Infrastructure/"]

COPY ["Core/Core.Common/Core.Common.csproj", "Core/Core.Common/"]

RUN dotnet restore "OrderService.Api/OrderService.Api.csproj"

COPY . .
COPY ../Core ../Core

WORKDIR "/OrderService/OrderService.Api"
RUN dotnet build "./OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Api.dll"]
