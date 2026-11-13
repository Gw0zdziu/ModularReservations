# ModularReservations

A learning project for **Domain-Driven Design**, **CQRS** and **message-based communication** in C# / .NET 10, built as a **modular monolith** around a cinema ticket reservation domain.

The domain was discovered with Event Storming. Board in Miro: [Board](https://miro.com/app/board/uXjVEQmpYLs=/?share_link_id=179729760873)

> **Status:** work in progress. The `Domain` layer of the `Reservations` module is implemented. Application, Infrastructure, API and the `Payments` / `Ticketing` modules are not implemented yet.

## Scope

The system covers the path from choosing a seat to a paid reservation and an issued ticket.

**In scope:** seat reservation, reservation lifecycle (pending, confirmed, cancelled, expired), payment, refund after cancellation, ticket issuing.

**Out of scope for now** (marked cyan on the board): movie catalog and screening scheduling, user registration, notifications (confirmation email), ticket scanning and ticket annulment after a cancelled reservation. Catalog and user data are expected to be seeded.

## Architecture

### Bounded contexts

| Module | Aggregates | Status |
|---|---|---|
| `Reservations` | `Screening` (with `Seat` entity), `Reservation` | Domain implemented |
| `Payments` | `Payment` | Planned |
| `Ticketing` | `Ticket` | Planned |

Each bounded context is a separate module. Modules communicate only through integration events, never by referencing each other.

### Solution structure

```
ModularReservations.slnx
└── src/
    ├── Bootstrapper/
    │   └── ModularReservations.Bootstrapper        ← ASP.NET Core host, single entry point
    ├── Modules/
    │   └── Reservations/
    │       ├── ModularReservations.Reservations.Domain
    │       ├── ModularReservations.Reservations.Application
    │       ├── ModularReservations.Reservations.Infrastructure
    │       └── ModularReservations.Reservations.Api
    └── Shared/
        └── ModularReservations.Shared              ← AggregateRoot, IEvent, DomainException
```

Every layer of every module is a separate class library, so the compiler enforces layer boundaries. Only `Bootstrapper` is a Web API project.

### Layer dependencies

```mermaid
flowchart LR
    Bootstrapper --> Api
    Bootstrapper --> Infrastructure
    Api --> Application
    Application --> Domain
    Infrastructure --> Application
    Infrastructure --> Domain
    Domain --> Shared
```

| Layer | Responsibility |
|---|---|
| **Domain** | Aggregates, entities, value objects, domain events, business rules. No framework dependencies. |
| **Application** | Command and query handlers, orchestration. Uses repository interfaces, never implementations. |
| **Infrastructure** | EF Core, repositories, message broker, external services, DI registration. |
| **Api** | Endpoints translating HTTP into commands and queries. No business logic. |
| **Shared** | Abstractions common to all modules. Contains no module-specific logic and references no module. |

## Domain model (Reservations module)

### `Screening` (aggregate root)

A single showing of a movie at a given time. Owns its seats and guards the main invariant: **a seat can be reserved only once per screening**.

- `Create(screeningDate, now, movieId, seatPositions)`: the date must be UTC and in the future, `movieId` must be set, and the seat positions must be non-empty and unique. The aggregate creates its own `Seat` entities, all free.
- `ReserveSeat(seatId, userId, reservationDate)`: validates that the date is set and UTC, `userId` is not empty and the seat exists. Rejected once the screening has started. Raises `SeatReservedEvent`.
- `ReleaseSeat(seatId)`: no time rule, because releasing a seat creates no obligation. Raises `SeatReleasedEvent`.

The movie is referenced only by `MovieId`.

### `Seat` (internal entity)

Identified by `Guid`, positioned by `SeatPosition`. Created and changed only through `Screening` (`internal` members). `Reserve()` rejects an already reserved seat and `Release()` rejects an already free one.

### `SeatPosition` (value object)

Row number and number in row, both greater than zero. Immutable record with value equality, used to detect duplicate positions.

### `Reservation` (aggregate root)

The lifecycle of a single seat reservation for a user. References the screening, seat and user by id.

- `Create(now, screeningId, reservationDate, seatId, userId)`: validates ids and the reservation date (UTC, not in the future, not older than 15 minutes, so the reservation is never created already expired). Starts as `Pending`, and the aggregate itself computes `ExpirationDate` (15 minutes after the reservation).
- `ConfirmReservation(now)`: only from `Pending`. Raises `ReservationConfirmedEvent`.
- `CancelReservation()`: only from `Confirmed`, i.e. the user changes their mind after paying. Raises `ReservationCancelledEvent`.
- `ExpireReservation(now)`: only from `Pending`, after `ExpirationDate`. Raises `ReservationExpiredEvent`.

Every method rejects a call from any other status with `InvalidReservationStatusException`.

```mermaid
stateDiagram-v2
    [*] --> Pending
    Pending --> Confirmed: payment completed
    Pending --> Expired: expiration date passed
    Confirmed --> Cancelled: user cancels after paying
    Expired --> [*]
    Cancelled --> [*]
```

A declined payment does not cancel the reservation: the user can pay again by another method until it expires. `Cancelled` and `Expired` are final states. `ReservationStatus` starts at `1`, so a default (`0`) value is never a valid status.

### Domain events

| Event | Raised by | Data |
|---|---|---|
| `SeatReservedEvent` | `Screening.ReserveSeat` | `ScreeningId`, `SeatId`, `UserId`, `ReservationDate` |
| `SeatReleasedEvent` | `Screening.ReleaseSeat` | `ScreeningId`, `SeatId` |
| `ReservationConfirmedEvent` | `Reservation.ConfirmReservation` | `ScreeningId`, `SeatId`, `ReservationId` |
| `ReservationCancelledEvent` | `Reservation.CancelReservation` | `ScreeningId`, `SeatId`, `ReservationId` |
| `ReservationExpiredEvent` | `Reservation.ExpireReservation` | `ScreeningId`, `SeatId`, `ReservationId` |

Aggregates inherit from `AggregateRoot` (in `Shared`), which collects events after a successful state change. Aggregates never publish events; the Application layer dispatches them after saving and then calls `ClearEvents()`.

### Design rules followed

- Aggregates are created only through static factory methods that validate every invariant. Constructors are private.
- State changes only through domain methods; setters are private.
- Aggregates reference each other only by id.
- The domain never reads the clock: the current time is passed in from outside.
- Event data comes from the aggregate's own state, not from the caller.
- One transaction changes one aggregate. Aggregates are coordinated by events (eventual consistency).

## Event Storming flows

Policies from the board that connect the aggregates:

```mermaid
flowchart TD
    A[User: Select screening] --> B[ScreeningSelected]
    B --> C[User: Reserve seat] --> D[SeatReserved]
    D -->|When a seat has been reserved| E[System: Create reservation] --> F[ReservationCreated]

    G[Background job every minute] --> H[System: Expire reservation] --> I[ReservationExpired]
    I -->|release the seat| R[System: Release seat] --> S[SeatReleased]

    L[User: Initialize payment] --> M[PaymentInitiated]
    M --> N[System: Prepare payment]
    N --> O[PaymentCompleted]
    N --> P[PaymentDeclined]
    P -.->|no reaction, the user can pay again| L

    O -->|confirm the booking| U[System: Confirm reservation] --> V[ReservationConfirmed]
    V -->|issue a ticket| X[System: Issue ticket] --> Y[TicketIssued]

    J[User: Cancel reservation] --> K[ReservationCancelled]
    K -->|release the seat| R
    K -->|refund payment| Z[System: Refund payment] --> PR[PaymentRefunded]
```

Cancelling is possible only after payment. A cancelled reservation releases the seat and refunds the payment. Annulling the already issued ticket is out of scope (cyan on the board).

Actors are `User` and `System`. The payment gateway is treated as part of `System`.

## Getting started

Requirements: [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet build ModularReservations.slnx
dotnet run --project src/Bootstrapper/ModularReservations.Bootstrapper
```

The host currently has no module endpoints wired in.

## Roadmap

- [x] Event Storming (events, commands, actors, policies, aggregates)
- [x] Solution structure and layer references
- [x] `Reservations` domain: `Screening`, `Seat`, `SeatPosition`, `Reservation`, domain events
- [x] `Reservation.CancelReservation()` allowed only from `Confirmed`
- [x] Domain exceptions: abstract `DomainException` in `Shared`, sealed specific exceptions in `Reservations.Domain/Exceptions`
- [ ] Unit tests for the domain
- [ ] `Reservations` Application layer: commands, queries, event handlers
- [ ] `Reservations` Infrastructure: EF Core, repositories, optimistic concurrency on aggregates
- [ ] Message broker and integration events between modules
- [ ] Background job expiring overdue reservations
- [ ] `Payments` module
- [ ] `Ticketing` module

### Open questions

- What happens when a payment completes after the reservation has already expired.
- How `Payment` allows another attempt after `PaymentDeclined`: a new `Payment` or the same aggregate returning to a waiting state.
- Annulling a ticket after the reservation is cancelled (out of scope for now).
- What releases a seat when `Screening.ReserveSeat` succeeded but `Reservation.Create` later fails for another reason, so that a seat is never reserved without a matching reservation.
