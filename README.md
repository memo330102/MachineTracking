# MACHINE MONITORING
This project is a .NET-based application that simulates receiving data from automotive machines (via MQTT), stores the data in a PostgreSQL database, and visualizes the data on a real-time dashboard. The project consists of multiple components, including services for data collection, SignalR for real-time updates, and a web-based Blazor dashboard using MudBlazor.
# Project Structure

**1. MessageBroker** 

A class library responsible for interacting with the External MQTT broker(like Mosquitto) and managing real-time data flow.

- **Features:**
  + **Subscription Service:**
  Subscribes to data from Mosquitto using MQTTnet. The received data is stored in the PostgreSQL database using Dapper.
  + **SignalR Service:**
  Publishes the subscribed data to clients in real time using SignalR.
- **Role in the Application:** This library acts as a hosted service in the API project.

**2. API** 

A .NET 8 Web API that facilitates communication between the Blazor WebAssembly client and the PostgreSQL database.
- **Features:**
  + Exposes endpoints to query and retrieve machine data stored in PostgreSQL.
  + Acts as a bridge for Blazor client requests and SignalR subscriptions.

**3. Client** 

A Blazor WebAssembly application for presenting machine data using a modern dashboard interface.
- **Features:**
  + Developed using MudBlazor for rich UI components.
  + Communicates with the API project via HttpClient.
  + Subscribes to real-time data updates from SignalR to display live data.
  + Dashboard includes:
       * A list of machines.
       * Real-time graphs and charts.

**4. Application** 

A class library containing business logic.
- **Role in the Application:**
   Acts as the intermediary between the API and Infrastructure projects. Implements core functionalities and rules.
  
**5. Infrastructure** 

A class library that handles all database interactions.
- **Features:**
    + Utilizes Dapper for efficient data access.
    + Only interacts with the Application project for business rules.
      
**6. Domain** 

A class library containing:
- Data Transfer Objects (DTOs).
- Interfaces defining core contracts for communication between layers.

# Setup Instructions
**Prerequisites**
- .NET 8 SDK
- PostgreSQL (Ensure the database is set up with the required schema)
- Mosquitto MQTT Broker (or any MQTT broker for simulation like HiveMQ , MQTTLab ,MQTT Explorer,MQTTBox,Postman (With MQTT Plugin))

# Steps to Run the Application
1. **Clone the Repository**
2.
    ```
    git clone https://github.com/memo330102/MachineTracking.git
    ```
4. **Setup the PostgreSQL Database**
5.    - Create a database named postgres.
      - Create the necessary schema and tables
 ```
CREATE TABLE public.machinehistory (
	id serial4 NOT NULL,
	machineid text NOT NULL,
	status text NOT NULL,
	chainmovespersecond float4 NOT NULL,
	articlenumber text NOT NULL,
	datareceivedtimestamp timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	statusid int4 NOT NULL,
	topic text NOT NULL,
	CONSTRAINT machinehistory_pkey PRIMARY KEY (id)
);
 ```
6. **Start the Mosquitto MQTT Broker**
7. **Build and Run the Solution**
8. **Run the Client Project**
9. **Access the Application**
