# ElevatorOS — Elevator Simulation System

A real-time console elevator simulation built in C# (.NET 10) demonstrating enterprise-level
backend architecture using Clean Architecture, CQRS, MediatR, and the SOLID principles.

## Overview

ElevatorOS simulates the control system of a multi-elevator building. The system
dispatches elevators in real-time, manages passenger boarding and disembarkation,
and renders the building state as a live ASCII frame in the console.

The project demonstrates:

- Clean Architecture with enforced layer boundaries via separate `.csproj` files
- CQRS implemented with MediatR — commands mutate state, queries read state
- A concurrent simulation engine using `System.Threading.Channels` to serialise
  state mutations without locks
- TDD from the domain layer outward — 40+ tests across domain, application, and
  infrastructure layers
- Structured logging via Serilog 

---

## Architecture

The solution follows Onion Architecture — dependency arrows point inward. Outer
layers know about inner layers; inner layers know nothing about outer layers.

- Presentation (Console entry point)
-  Infrastructure (repos, services, DI) 
- Application (CQRS handlers, behaviours) 
- TDD from the domain layer outward — 40+ tests across domain, application, and
  infrastructure layers
-  Domain (entities, enums, interfaces) 


### Key patterns

**CQRS via MediatR** — every operation is either a command (mutates state, returns
`Unit`) or a query (reads state, returns a DTO). All commands are serialised through
a bounded `Channel<IBaseRequest>` to eliminate write-side race conditions.

**Pipeline behaviours** — `LoggingBehaviour`  run automatically for every request.

**Dispatch algorithm** — `NearestElevatorStrategy` scores elevators by distance,
direction compatibility, and capacity. Idle elevators (Stationary direction) match
any passenger direction as a wildcard. Elevators heading the wrong way are
deprioritised to a fallback tier.

**Background services** — three `BackgroundService` implementations run concurrently:
`SimBackgroundService` (500ms tick), `PassengerGeneratorService` (configurable
interval), and `RenderLoopService` (200ms render).

---

## Project Structure

```
ElevatorSimulation/
├── src/
│   ├── Domain/                         # Entities, enums, interfaces — no dependencies
│   │   ├── Entities/
│   │   │   ├── Elevators/
│   │   │   │   ├── ElevatorBase.cs
│   │   │   │   ├── PassengerElevator.cs
│   │   │   │   ├── GlassElevator.cs
│   │   │   │   ├── HighSpeedElevator.cs
│   │   │   │   └── ServiceElevator.cs
│   │   │   ├── BuildingControl.cs
│   │   │   ├── Floor.cs
│   │   │   └── Passenger.cs
│   │   ├── Enums/
│   │   ├── Exceptions/
│   │   ├── Factory/ # elvator factory design pattern
│   │   └── Interfaces/
│   │
│   ├── Application/                    # CQRS handlers, behaviours, DTOs
│   │   ├── Behaviours/
│   │   │   └── LoggingBehaviour.cs
│   │   ├── Commands/
│   │   │   ├── RequestElevator/
│   │   │   ├── RegisterPassenger/
│   │   │   ├── MoveElevator/
│   │   │   ├── BoardPassengers/
│   │   │   └── DisembarkPassengers/
│   │   ├── Queries/
│   │   │   ├── GetSystemStatus/
│   │   │   └── GetElevatorState/
│   │   ├── Dispatching/
│   │   │   └── NearestElevatorStrategy.cs
│   │   └── DTOs/
│   │
│   ├── Infrastructure/                 # Implementations, services, console
│   │   ├── Repositories/
│   │   │   ├── InMemoryElevatorRepo.cs
│   │   │   └── InMemoryFloorRepo.cs
│   │   ├── Config/ # config for the sim engine (floors etc)
│   │   │   ├── PassengerGeneration.cs
│   │   │   └── SimOptions.cs
│   │   ├── Services/
│   │   │   ├── SimBackgroundService.cs
│   │   │   ├── CommandProcessorService.cs
│   │   │   ├── PassengerGeneratorService.cs
│   │   │   └── RenderLoopService.cs
│   │   ├── Console/
│   │   │   ├── ConsoleRenderer.cs
│   │   │   ├── InputHandler.cs
│   │   │   └── Parsing/
│   │   ├── Logging/
│   │   │   ├── SerilogConfig.cs
│   │   │   └── InMemoryLogSink.cs
│   │   └── DependencyInjection.cs
│   │
│   └── Presentation/                   # Entry point only
│       └── Program.cs
│
├── tests/
│   ├── Domain.Tests/
│   ├── Application.Tests/
│   └── Infrastructure.Tests/
│
├── docs/ #TODO:  Future improvements and additions
│   ├── RFC-001-Architecture.md
│   └── RFC-002-DispatchAlgorithm.md
│
└── README.md
```

---

## Prerequisites

| Requirement | Version  |
|-------------|----------|
| .NET SDK    | 10.0+    |
| Git         | Any      |
| Terminal    | macOS / Linux / Windows (with ANSI support) |

Verify your .NET version:

```bash
dotnet --version
```

---

## Setup and Installation

```bash
# 1. Clone the repository
git clone https://github.com/Graduate-Program-26/dotnet-elevator-lebogang.git
cd dotnet-elevator-lebogang

# 2. Restore all dependencies
dotnet restore ./dotnet_elevator_lebogang.slnx

# 3. Build the solution
dotnet build ./dotnet_elevator_lebogang.slnx
```

---

## Running the Application

```bash
dotnet run --project src/Presentation
```

The console will clear and display the live ASCII building frame. The simulation
starts automatically — elevators idle on floor 0, passengers generated every 3
seconds by default.

**Windows users** — enable ANSI escape codes if the frame characters appear
corrupted:

```powershell
# Run once in PowerShell before starting the app
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
```

---

## Running the Tests

```bash
# Run all tests
dotnet test ./dotnet_elevator_lebogang.slnx --verbosity normal

# Run a specific layer
dotnet test tests/Domain.Tests --verbosity normal
dotnet test tests/Application.Tests --verbosity normal
dotnet test tests/Infrastructure.Tests --verbosity normal

# Run a specific test class
dotnet test tests/Application.Tests \
  --filter "FullyQualifiedName~NearestElevatorStrategyTests" \
  --verbosity normal
```

Test logs are written to `**/*.trx` and published as GitHub Actions test reports
on every pull request.

---

## Console Commands

Type commands in the input row at the bottom of the frame and press **Enter**.

| Command | Description | Example |
|---------|-------------|---------|
| `call [floor] [up\|down] [destination]` | Call elevator as a passenger | `call 3 up 7` |
| `status` | Show full building snapshot | `status` |
| `status [elevator-id]` | Show detailed view of one elevator | `status <guid>` |
| `sim on` | Enable automatic passenger generation | `sim on` |
| `sim off` | Disable automatic passenger generation | `sim off` |
| `quit` | Shut down the simulation gracefully | `quit` |

**Backspace** removes the last character. **Escape** clears the input buffer.

---

## Design Decisions and Trade-offs
> Will be added in future iterations and improvements in the docs folder

### Summary of key decisions

**CQRS without a database** — MediatR was chosen not for read/write store separation
but for three specific benefits: automatic cross-cutting concerns via pipeline
behaviours (logging), enforced separation of read
intent from write intent, and handler isolation that makes each operation
independently testable. The in-memory repository acts as the single source of truth
for the simulation lifetime.

**`Channel<IBaseRequest>` for concurrency** — all state-mutating commands are
serialised through a single bounded channel with one consumer
(`CommandProcessorService`). This eliminates write-side race conditions without
locks. Queries bypass the channel because `ConcurrentDictionary` makes reads
thread-safe. At larger scale the channel would be replaced by an event bus— `ICommandQueue` is the only seam that would change.


**Passenger registration split from elevator dispatch** — `RegisterPassengerCommand`
and `RequestElevatorCommand` are separate commands. The `PassengerGeneratorService`
registers passengers directly via `IFloorRepo` before writing a
`RequestElevatorCommand` to the channel. User-initiated calls send both commands
sequentially. This prevents duplicate passenger registration that would occur if the
handler always created a passenger regardless of source.

**Dispatch direction matching** — stationary elevators match any passenger direction
(wildcard) and are preferred over elevators travelling the wrong way. The
`WillPassFloor()` check ensures a travelling elevator is only considered if the
requested floor lies in the path of its current trajectory.

---

## Assumptions

- Ground floor is floor `0`. Floors are zero-indexed.
- Passengers declare their destination when calling the elevator (at console input
  time), not after boarding. This is a simplification — real elevators allow
  destination selection inside the car.
- The dispatch algorithm uses a nearest-first strategy with direction matching. It
  does not implement look-ahead scheduling (SCAN/LOOK algorithm), which would be
  the next improvement for high-traffic scenarios.
- Automatic passenger generation picks destinations randomly with uniform
  distribution.
- All elevator state is held in memory and does not persist across restarts. This is
  appropriate for a simulation; a production system would use a persistent store.
- The `ServiceElevator` type is excluded from passenger dispatch by the
  `NearestElevatorStrategy`. It exists to demonstrate the OCP — new elevator types
  can be added without modifying dispatch logic.
- Maximum elevator capacity is fixed per elevator type at the factory level and does not change at
  runtime.

---

## CI/CD

GitHub Actions runs on every push to `main` and `development`, and on every pull
request targeting `main`.

The pipeline:

1. Restores dependencies
2. Builds the solution in Release configuration
3. Runs all tests and publishes results as a PR test report

---

## Elevator Types

| Type | Capacity | Speed | Dispatch eligible |
|------|----------|-------|-------------------|
| `PassengerElevator` | 8 | Standard (1 floor/tick) | Yes |
| `GlassElevator` | 6 | Standard | Yes |
| `HighSpeedElevator` | 12 | Fast (2 floors/tick) | Yes |
| `ServiceElevator` | 4 | Standard | No — freight only |

---
