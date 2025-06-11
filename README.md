# ServicePlanner

A comprehensive church service planning application built with ASP.NET Core 8.0 and Blazor Server. ServicePlanner helps churches organize worship services, manage song libraries, create service templates, and coordinate service planning activities.

## Features

- **Service Management**: Create, edit, and manage worship services
- **Song Library**: Maintain a comprehensive database of songs with metadata (key, tempo, category, etc.)
- **Service Templates**: Create reusable service templates for different types of worship services
- **User Management**: Role-based access control with Admin and User roles
- **Authentication**: Secure login system with ASP.NET Core Identity
- **Import Functionality**: Bulk import songs from various sources
- **Responsive Design**: Works on desktop and mobile devices

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **UI**: Blazor Server Components
- **Database**: SQLite with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Styling**: Bootstrap 5
- **Password Hashing**: BCrypt.Net

## Quick Start with Docker

The easiest way to run ServicePlanner is using Docker:

```bash
# Pull and run the latest image
docker run -p 8080:8080 serviceplanner:latest

# Or run with persistent database storage
docker run -p 8080:8080 -v $(pwd)/data:/app/data serviceplanner:latest
```

Access the application at `http://localhost:8080`

**Default Admin Credentials:**
- Email: `admin@serviceplanner.com`
- Password: `Admin123!`

## Docker Deployment Options

### Basic Deployment
```bash
docker run -p 8080:8080 serviceplanner:latest
```

### With Persistent Database
```bash
# Create a data directory
mkdir data

# Run with volume mount for database persistence
docker run -p 8080:8080 -v $(pwd)/data:/app/data serviceplanner:latest
```

### Using Docker Compose (Recommended)
```bash
# Clone the repository
git clone https://github.com/highphilosopher/ServicePlanner.git
cd ServicePlanner

# Start the application
docker-compose up -d
```

### Custom Database Location
```bash
# Set custom database path via environment variable
docker run -p 8080:8080 \
  -e SERVICEPLANNER_DB_PATH=/app/custom/serviceplanner.db \
  -v $(pwd)/custom:/app/custom \
  serviceplanner:latest
```

## Automated Docker Publishing

This repository includes a GitHub Actions workflow that automatically builds and publishes Docker images to Docker Hub when pull requests are merged to the main branch.

### Setup Requirements

To enable automated Docker publishing, you need to configure the following secrets in your GitHub repository:

1. Go to your GitHub repository → Settings → Secrets and variables → Actions
2. Add the following repository secrets:

| Secret Name | Description |
|-------------|-------------|
| `DOCKERHUB_USERNAME` | Your Docker Hub username |
| `DOCKERHUB_TOKEN` | Docker Hub access token (not password) |

### Creating a Docker Hub Access Token

1. Log in to [Docker Hub](https://hub.docker.com/)
2. Go to Account Settings → Security
3. Click "New Access Token"
4. Give it a descriptive name (e.g., "GitHub Actions ServicePlanner")
5. Copy the generated token and add it as `DOCKERHUB_TOKEN` secret

### Workflow Behavior

The GitHub Action will:
- Trigger on pushes to `main` branch
- Trigger on merged pull requests to `main` branch
- Build multi-platform images (linux/amd64, linux/arm64)
- Tag images with:
  - `latest` (for main branch)
  - Branch name
  - Git SHA
- Use GitHub Actions cache for faster builds
- Push to `your-dockerhub-username/serviceplanner`

**Note**: The project has been updated to resolve build issues that were preventing successful Docker image creation in GitHub Actions. Key fixes include:
- Removed Docker HEALTHCHECK dependency on `curl` (not available in .NET runtime images)
- Fixed Song model initialization in database seeding to include all required properties
- Updated Dockerfile to build from solution level to ensure proper assembly references on Linux
- Downgraded Entity Framework Core packages from 9.0.5 to 8.0.11 to match .NET 8.0 compatibility
- Added Entity Framework Core tools installation in Dockerfile for Linux build compatibility
- Ensured cross-platform compatibility for Linux-based GitHub Actions runners

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `SERVICEPLANNER_DB_PATH` | Path to SQLite database file | `./data/serviceplanner.db` |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | `Production` |
| `ASPNETCORE_URLS` | URLs the app listens on | `http://+:8080` |

## Local Development

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- Git

### Setup
```bash
# Clone the repository
git clone https://github.com/highphilosopher/ServicePlanner.git
cd ServicePlanner

# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update --project ServicePlanner

# Run the application
dotnet run --project ServicePlanner
```

The application will be available at `https://localhost:7001` or `http://localhost:5000`.

### Database Migrations
```bash
# Add a new migration
dotnet ef migrations add MigrationName --project ServicePlanner

# Update database
dotnet ef database update --project ServicePlanner

# Remove last migration
dotnet ef migrations remove --project ServicePlanner
```

## Building Docker Image

### Build Locally
```bash
# Build the Docker image
docker build -t serviceplanner:latest .

# Run the built image
docker run -p 8080:8080 serviceplanner:latest
```

### Build Scripts
Use the provided build scripts for convenience:

**Windows:**
```cmd
build-docker.bat
```

**Linux/macOS:**
```bash
chmod +x build-docker.sh
./build-docker.sh
```

## Project Structure

```
ServicePlanner/
├── Components/           # Blazor components and pages
│   ├── Layout/          # Layout components
│   └── Pages/           # Page components
├── Data/                # Entity Framework context
├── Migrations/          # EF Core migrations
├── Models/              # Data models
├── Services/            # Business logic services
├── Properties/          # Launch settings
└── wwwroot/            # Static web assets
```

## Key Components

- **ServicePlannerContext**: Entity Framework database context
- **User Management**: ASP.NET Core Identity integration
- **Service Templates**: Reusable service structures
- **Song Library**: Comprehensive song database with metadata
- **Role-Based Access**: Admin and User role management

## Default Data

The application seeds with:
- Sample songs across different categories (Hymns, Contemporary, Christmas)
- Default service template for Sunday morning worship
- Admin user account for initial setup

## Security Features

- Password requirements (8+ characters, mixed case, numbers, symbols)
- Account lockout after failed attempts
- Secure cookie authentication
- Role-based authorization
- HTTPS redirection in production

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support, please open an issue on GitHub or contact the development team.

## Changelog

### v1.0.0
- Initial release
- Basic service planning functionality
- Song library management
- User authentication and authorization
- Docker containerization support
