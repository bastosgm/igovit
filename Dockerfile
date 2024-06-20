# Stage 1: Build Angular App
FROM node:16 AS nodebuild
WORKDIR /app
EXPOSE 8080

# Install Node.js and npm
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_22.x | bash - && \
    apt-get install -y nodejs

# Copy Angular project files and install dependencies
COPY igovit-client/package*.json ./
RUN npm install
COPY igovit-client/. ./

# Build the Angular application
RUN npm run build --prod

# Stage 2: Build ASP.NET Core App
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy and restore .NET project files
COPY *.csproj ./
RUN dotnet restore

# Copy all files and build the .NET project
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Build Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy .NET publish output
COPY --from=build /app/publish ./

# Copy Angular build output to wwwroot
COPY --from=nodebuild /app/dist /app/wwwroot

# Copy SSL certificate and key
COPY certs/aspnetapp.pfx /root/.aspnet/https/aspnetapp.pfx

# Expose the port your application will run on
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Production

# Set the entry point to the .NET application
ENTRYPOINT ["dotnet", "igovit.dll"]
