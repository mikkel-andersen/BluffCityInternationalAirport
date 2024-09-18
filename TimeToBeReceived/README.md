# TimeToBeReceived

This project demonstrates how to use RabbitMQ to send and receive messages with a time-to-live (TTL) setting.

## Project Structure
TimeToBeReceived/ ├── Program.cs ├── Producers/ │ └── Producer.cs ├── Consumers/ │ └── Consumer.cs ├── TimeToBeReceived.csproj ├── README.md

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [RabbitMQ](https://www.rabbitmq.com/download.html)

### Running the Application

1. **Start RabbitMQ Server**:
   Ensure RabbitMQ server is running on `localhost`.

2. **Build and Run the Application**:
   Navigate to the project directory and run the following commands:

   ```sh
   dotnet build
   dotnet run
   
