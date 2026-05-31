FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SmartNotes.Core/SmartNotes.Core.csproj SmartNotes.Core/
COPY SmartNotes.Api/SmartNotes.Api.csproj SmartNotes.Api/
RUN dotnet restore SmartNotes.Api/SmartNotes.Api.csproj

COPY SmartNotes.Core/ SmartNotes.Core/
COPY SmartNotes.Api/ SmartNotes.Api/
RUN dotnet publish SmartNotes.Api/SmartNotes.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SmartNotes.Api.dll"]
EOF

cat > SmartNotes.Worker/Dockerfile << 'EOF'
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SmartNotes.Core/SmartNotes.Core.csproj SmartNotes.Core/
COPY SmartNotes.Worker/SmartNotes.Worker.csproj SmartNotes.Worker/
RUN dotnet restore SmartNotes.Worker/SmartNotes.Worker.csproj

COPY SmartNotes.Core/ SmartNotes.Core/
COPY SmartNotes.Worker/ SmartNotes.Worker/
RUN dotnet publish SmartNotes.Worker/SmartNotes.Worker.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SmartNotes.Worker.dll"]