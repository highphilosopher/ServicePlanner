# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install Entity Framework Core tools and set PATH
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy solution and project files
COPY ServicePlanner.sln ./
COPY ServicePlanner/*.csproj ./ServicePlanner/

# Restore dependencies
RUN dotnet restore ServicePlanner.sln

# Copy all source code
COPY ServicePlanner/ ./ServicePlanner/

# Build the application
RUN dotnet build ServicePlanner.sln -c Release

# Publish the application
RUN dotnet publish ServicePlanner/ServicePlanner.csproj -c Release -o /app/publish /p:UseAppHost=false

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

# Health check removed to avoid dependency issues in Linux containers
# You can add health checks at the orchestration level (Docker Compose, Kubernetes, etc.)

# Set the entry point
ENTRYPOINT ["dotnet", "ServicePlanner.dll"]
