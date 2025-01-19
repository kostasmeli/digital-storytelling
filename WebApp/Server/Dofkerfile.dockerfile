# Use the .NET 9.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the entire solution
COPY . ./

# Set the working directory to the server project
WORKDIR /app/Server

# Restore dependencies
RUN dotnet restore

# Build the application in Release mode
RUN dotnet publish -c Release -o /publish

# Use the .NET 9.0 runtime image for production
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /publish .

# Expose the default port for ASP.NET Core
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "BlazorApp.Server.dll"]
