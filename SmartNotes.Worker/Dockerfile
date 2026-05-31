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