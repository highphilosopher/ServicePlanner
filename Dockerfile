# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ServicePlanner.sln ./
COPY ServicePlanner/*.csproj ./ServicePlanner/
RUN dotnet restore ServicePlanner/ServicePlanner.csproj

# Copy source code and build the application
COPY . .
WORKDIR /src/ServicePlanner
RUN dotnet build ServicePlanner.csproj -c Release -o /app/build

# Publish the application
RUN dotnet publish ServicePlanner.csproj -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8.0 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create a non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Create data directory and set permissions
RUN mkdir -p /app/data && chown -R appuser:appuser /app/data

# Copy the published application
COPY --from=build /app/publish .

# Set ownership of the application files
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port 8080
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV SERVICEPLANNER_DB_PATH=/app/data/serviceplanner.db

# Add health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set the entry point
ENTRYPOINT ["dotnet", "ServicePlanner.dll"]
