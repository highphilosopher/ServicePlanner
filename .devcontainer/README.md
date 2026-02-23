# Dev Container Setup

This project includes a dev container configuration to provide a consistent development environment.

## What's Included

The dev container provides:
- .NET 9.0 SDK for building and running the application
- Node.js LTS for frontend tooling (if needed)
- VS Code extensions for C# development
- Pre-configured ports for the application (5000, 5001)
- Docker integration

## How to Use

1. Open this project in Visual Studio Code
2. When prompted, select "Reopen in Container" or manually run:
   - `Ctrl+Shift+P` to open the command palette
   - Type "Dev Containers: Reopen in Container"
3. The container will build automatically and install dependencies
4. Once ready, you can start the application with:
   ```bash
   dotnet run
   ```

## Configuration Details

The devcontainer.json file includes:
- Base image: `mcr.microsoft.com/dotnet/aspnet:9.0`
- Features: .NET SDK and Node.js LTS
- VS Code extensions for C# development
- Port forwarding for the application (5000, 5001)
- NuGet package caching for faster builds

## Environment Variables

The container will automatically mount the necessary directories for:
- NuGet packages
- Database files in the `./data` directory