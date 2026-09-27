FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY API/API.csproj API/
COPY Service/Service.csproj Service/
COPY Infra/Infra.csproj Infra/

RUN dotnet restore API/API.csproj

COPY . .

RUN dotnet publish API/API.csproj -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /out ./

RUN mkdir -p /app/data && chown app:app /app/data

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080

USER app

EXPOSE 8080

ENTRYPOINT ["dotnet", "API.dll"]