# MiniProjekt

## Overview

This project demonstrates a message processing system using RabbitMQ. It includes a producer that sends messages, consumers that process these messages, a resequencer that orders the messages, and an aggregator that combines them.

## Prerequisites

- .NET 8.0 SDK
- RabbitMQ Server

## Setup

1. **Install RabbitMQ**:
    - Follow the instructions on the [RabbitMQ website](https://www.rabbitmq.com/download.html) to install RabbitMQ.
    - 
## Running the Application

1. **Start RabbitMQ Server**:
   Ensure RabbitMQ server is running on `localhost`.

2. **Run the application**:
   ```sh
   dotnet run
   ```

## Project Structure

- `Program.cs`: Entry point of the application. Sets up RabbitMQ connections and starts the producer, consumers, resequencer, and aggregator.
- `Producer.cs`: Reads XML files and sends messages to RabbitMQ queues.
- `Consumer.cs`: Consumes messages from RabbitMQ queues and sends them to the resequencer.
- `Resequencer.cs`: Orders the messages based on sequence numbers and sends them to the output queue.
- `Aggregator.cs`: Combines the ordered messages and prints them.

## XML File Structure

The XML files should follow this structure:

```xml
<FlightDetailsInfoResponse>
    <Flight number="SK937" Flightdate="20220225">
        <Origin>ARLANDA</Origin>
        <Destination>LONDON</Destination>
    </Flight>
    <Passenger>
        <ReservationNumber>CA937200305252</ReservationNumber>
        <FirstName>Anders</FirstName>
        <LastName>And</LastName>
    </Passenger>
    <Luggage>
        <Id>CA937200305252</Id>
        <Identification>1</Identification>
        <Category>Normal</Category>
        <Weight>17.3</Weight>
    </Luggage>
    <Luggage>
        <Id>CA937200305252</Id>
        <Identification>2</Identification>
        <Category>Large</Category>
        <Weight>22.7</Weight>
    </Luggage>
</FlightDetailsInfoResponse>
```