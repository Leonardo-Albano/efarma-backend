# Stage 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore dependencies
COPY ["EFarma/EFarma.csproj", "EFarma/"]
RUN dotnet restore "EFarma/EFarma.csproj"

# Copy and build
COPY ["EFarma", "EFarma/"]
RUN dotnet build "EFarma/EFarma.csproj" -c Release -o /app/build

# Stage 2: Publish Stage
FROM build AS publish
RUN dotnet publish "EFarma/EFarma.csproj" -c Release -o /app/publish

# Stage 3: Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app

# Copy published files and XML documentation for Swagger
COPY --from=publish /app/publish .
COPY --from=publish /app/publish/EFarma.xml .

# Set entry point for the application
ENTRYPOINT [ "dotnet", "EFarma.dll" ]
