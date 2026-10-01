# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project files first
COPY ["expense-tracker.api/expense-tracker.api.csproj", "expense-tracker.api/"]

# Restore dependencies
RUN dotnet restore "expense-tracker.api/expense-tracker.api.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/expense-tracker.api"

RUN dotnet publish "expense-tracker.api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# Render expects the web service to listen on port 10000
ENV ASPNETCORE_URLS=http://0.0.0.0:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "expense-tracker.api.dll"]