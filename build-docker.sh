#!/bin/bash

# ServicePlanner Docker Build Script
# This script builds the Docker image for ServicePlanner

set -e

echo "Building ServicePlanner Docker image..."

# Build the Docker image
docker build -t serviceplanner:latest .

echo "Docker image built successfully!"
echo ""
echo "To run the application:"
echo "  docker run -p 8080:8080 serviceplanner:latest"
echo ""
echo "To run with persistent database:"
echo "  docker run -p 8080:8080 -v \$(pwd)/data:/app/data serviceplanner:latest"
echo ""
echo "To run with docker-compose:"
echo "  docker-compose up -d"
echo ""
echo "Access the application at: http://localhost:8080"
