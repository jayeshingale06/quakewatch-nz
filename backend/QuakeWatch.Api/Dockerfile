# Build stage: full SDK, discarded afterwards.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Restore against the project file alone so the layer stays cached when only
# source changes.
COPY QuakeWatch.Api.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage: no SDK, no source.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app
COPY --from=build /app/publish .

# The app runs as a non-root user and cannot write into root-owned /app. With
# no PostgreSQL connection string it falls back to SQLite, so it needs a
# directory it is allowed to write to.
# The directory is inside the container and disappears on restart. That is fine:
# the database is only a cache of GeoNet and every row can be refetched.
RUN mkdir -p /app/data && chown -R $APP_UID /app/data
ENV Sqlite__Path=/app/data/quakewatch.db

# The aspnet base image already listens on 8080. Setting ASPNETCORE_URLS here
# only makes ASP.NET warn about an override. EXPOSE is documentation.
EXPOSE 8080

# Startup logs "Cannot load library libgssapi_krb5.so.2": Npgsql probes for
# Kerberos in case the connection string wants integrated auth. Ours uses a
# username and password, so the probe fails harmlessly. Installing the library
# would add weight and attack surface for an unused feature, so leave it.

# Not root. A compromise then lands on an account with almost no permissions.
USER $APP_UID

ENTRYPOINT ["dotnet", "QuakeWatch.Api.dll"]
