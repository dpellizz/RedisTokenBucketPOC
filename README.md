# RedisTokenBucketPOC

A proof-of-concept (POC) .NET console application demonstrating the Token Bucket algorithm using Redis as a backend for distributed rate limiting.

## Features
- Implements the Token Bucket algorithm
- Uses StackExchange.Redis for Redis integration
- Supports configurable bucket capacity, refill rate, and worker threads

## Requirements
- .NET 9.0 SDK or compatible
- Redis server (local or remote)

## Getting Started

### 1. Clone the repository
```
git clone <your-repo-url>
cd RedisTokenBucketPOC
```

### 2. Install dependencies
Make sure you have the .NET SDK and Redis installed and running locally.

### 3. Build the project
```
dotnet build
```

### 4. Run the project
```
dotnet run
```

## Configuration
- The Redis connection string is set to `localhost` by default in `Program.cs`.
- Adjust the token bucket parameters (capacity, refill rate, etc.) in `Program.cs` as needed.


## Running with Docker

You can run both the application and Redis using Docker Compose. This is the recommended way for local development and testing.

### 1. Build and start the containers
```sh
docker compose up --build
```

This will build the .NET app image and start both the app and a Redis server. The app will automatically connect to the Redis container using the hostname `redis`.

### 2. Stopping the containers
Press `Ctrl+C` to stop, then run:
```sh
docker compose down
```

### Notes
- Make sure your connection string in `Program.cs` is set to `redis` (not `localhost`) when running in Docker Compose.
- You need Docker and Docker Compose (or Docker Desktop) installed. On macOS, you can use Docker Desktop or Colima.

## License
MIT
