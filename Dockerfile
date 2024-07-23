FROM node:20 AS nodebuild
WORKDIR /app
EXPOSE 8080

RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_22.x | bash - && \
    apt-get install -y nodejs

COPY igovit-client/package*.json ./
RUN npm install
COPY igovit-client/. ./

RUN npm run build --prod

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

COPY --from=nodebuild /app/dist /app/wwwroot

COPY certs/private.pfx /root/.aspnet/https/private.pfx

EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "igovit.dll"]
