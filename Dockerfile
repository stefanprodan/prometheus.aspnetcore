FROM microsoft/dotnet:latest

# Set environment variables
ENV ASPNETCORE_URLS="http://*:5000"
ENV ASPNETCORE_ENVIRONMENT="Production"

# Copy files to app directory
COPY /src/Prometheus.AspNetCore /app/src/Prometheus.AspNetCore
COPY /test/Prometheus.Demo /app/src/Prometheus.Demo

# Set working directory
WORKDIR /app

# Restore Docker.DotNet packages
WORKDIR /app/src/Prometheus.AspNetCore
RUN ["dotnet", "restore"]

# Set working directory
WORKDIR /app/src/Prometheus.Demo

# Restore packages
RUN ["dotnet", "restore"]

# Build
RUN ["dotnet", "build"]

# Open port
EXPOSE 5000/tcp

# Run the app
ENTRYPOINT ["dotnet", "run"]