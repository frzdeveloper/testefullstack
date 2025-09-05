# Use the official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy solution file
COPY *.sln ./

# Copy project files
COPY UserManagement.Domain/UserManagement.Domain.csproj ./UserManagement.Domain/
COPY UserManagement.Application/UserManagement.Application.csproj ./UserManagement.Application/
COPY UserManagement.Infrastructure/UserManagement.Infrastructure.csproj ./UserManagement.Infrastructure/
COPY UserManagement.API/UserManagement.API.csproj ./UserManagement.API/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . ./

# Build the application
RUN dotnet build -c Release --no-restore

# Publish the application
RUN dotnet publish UserManagement.API/UserManagement.API.csproj -c Release -o out --no-build

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory
WORKDIR /app

# Copy the published application
COPY --from=build /app/out .

# Expose port
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "UserManagement.API.dll"]
