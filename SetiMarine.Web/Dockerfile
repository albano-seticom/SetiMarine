FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SetiMarine.Web/SetiMarine.Web.csproj", "SetiMarine.Web/"]
COPY ["SetiMarine.Domain/SetiMarine.Domain.csproj", "SetiMarine.Domain/"]
COPY ["SetiMarine.Application/SetiMarine.Application.csproj", "SetiMarine.Application/"]
COPY ["SetiMarine.Infrastructure/SetiMarine.Infrastructure.csproj", "SetiMarine.Infrastructure/"]
RUN dotnet restore "SetiMarine.Web/SetiMarine.Web.csproj"
COPY . .
WORKDIR "/src/SetiMarine.Web"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SetiMarine.Web.dll"]
