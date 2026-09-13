# ============================================================
# Stage 1: BUILD. Has the whole .NET SDK. Big, and thrown away.
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy ONLY the project file first, then restore.
# Docker caches this layer, so libraries are only downloaded
# again when the project file itself changes.
COPY QuakeWatch.Api.csproj .
RUN dotnet restore

# Now copy the code. Editing code does not invalidate the
# restore layer above.
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# ============================================================
# Stage 2: RUN. Only the runtime. Much smaller.
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app
COPY --from=build /app/publish .

# The aspnet base image already defaults to port 8080, so setting
# ASPNETCORE_URLS here is redundant and makes ASP.NET log a warning
# about overriding it. EXPOSE is documentation only.
EXPOSE 8080

# Note on the startup message "Cannot load library libgssapi_krb5.so.2":
# Npgsql probes for Kerberos support in case the connection string asks
# for integrated authentication. Ours uses a username and password, so
# the probe fails harmlessly and Npgsql carries on. Installing the
# Kerberos library would silence the message but add weight and attack
# surface for a feature we do not use, so it is left alone deliberately.

# Do not run as root. If the app is ever compromised, the
# attacker gets an account with almost no permissions.
USER $APP_UID

ENTRYPOINT ["dotnet", "QuakeWatch.Api.dll"]
