# CLAUDE.md

ModularReservations is a learning project: a cinema ticket reservation system in C# / .NET 10, built as a **modular monolith** with DDD, CQRS and message-based communication between modules. See `README.md` for the full domain description and roadmap.

## How to work with me (read first)

- **The owner writes the code.** Do not write production code, tests or fixes on your own initiative. When asked to review, evaluate only (DDD correctness, invariants, naming, boundaries). Give suggestions or implementations only when explicitly asked.
- Answer in **Polish**, concisely and factually. Code, identifiers and commit messages stay in English.
- If the owner is right, confirm it in one short sentence and move on. If the owner is wrong, say so directly, explain why and show the facts. Do not change your position without new evidence, and do not over-apologize.
- Do not review things that were not asked about (e.g. unused parameters when the question is about DDD), but list them as a note when you notice them.
- Deferred work goes to the `Open items` section at the end of this file, not just into the chat.

## Commands

```bash
dotnet build ModularReservations.slnx
dotnet run --project src/Bootstrapper/ModularReservations.Bootstrapper
dotnet test ModularReservations.slnx
```

## Architecture

- Solution file: `ModularReservations.slnx`. Sources in `src/`, tests (xUnit) in `tests/`, mirroring `src/` (so far `Reservations.Domain.Tests`).
- `src/Bootstrapper`: the only Web API host; registers `IDispatcher` as scoped in `Program.cs`. `src/Modules/<Module>`: one module per bounded context, one `csproj` per layer: `Domain`, `Application`, `Infrastructure`, `Api`.
- `src/Shared` has two projects:
  - `Shared.Abstraction` (root namespace `ModularReservations.Shared`, no dependencies): `AggregateRoot`, `DomainException`, `ICommand`/`ICommandHandler`, `IEvent`/`IEventHandler`, `IDispatcher`.
  - `Shared.Infrastructure` (→ `Shared.Abstraction`, `Microsoft.Extensions.DependencyInjection.Abstractions`): `Dispatcher`. `SendAsync` resolves exactly one `ICommandHandler<T>`; `PublishAsync` resolves handlers by the event's runtime type (`@event.GetType()`), so publishing from `aggregate.Events` (`IEvent`) works, and awaits them sequentially.
- Allowed references: Bootstrapper → Api, Infrastructure, Shared.Infrastructure; Api → Application; Application → Domain; Infrastructure → Application, Domain; Domain → Shared.Abstraction; Shared.Infrastructure → Shared.Abstraction. `Shared` references no module. Modules never reference each other; they communicate only through integration events.
- Modules: `Reservations` (aggregates `Screening` with `Seat` entity, `Reservation`; domain implemented), `Payments` (`Payment`, planned), `Ticketing` (`Ticket`, planned). Application, Infrastructure and Api layers are still empty.
- Event Storming board (source of truth for flows): https://miro.com/app/board/uXjVEQmpYLs=/ . Cyan stickies are out of scope.

## Domain conventions

- Aggregates are created only through static factory methods (`Create`) that validate every invariant. Constructors are private; setters are private; state changes only through domain methods.
- Aggregates reference each other by `Guid` id only. One transaction changes one aggregate; aggregates are coordinated by events (eventual consistency).
- The domain never reads the clock. Current time is passed in as `now`. All dates are `DateTime` with `Kind = Utc` (use `TimeProvider.GetUtcNow().UtcDateTime` in Application).
- Domain events are added with `AddEvent` after a successful state change, carry data taken from the aggregate's own state, and are dispatched by the Application layer after saving, followed by `ClearEvents()`.
- Errors are exceptions deriving from the abstract `DomainException` (in `Shared`). Each concrete exception is `sealed`, lives in `<Module>.Domain/Exceptions` (subfolders per aggregate; exceptions used by both aggregates stay in the root), exposes its data as properties and builds its own message. Do not throw `ArgumentException` or `InvalidOperationException` from the domain.
- Repository interfaces live in `Domain`; implementations in `Infrastructure`.
- `ReservationStatus` starts at 1; default (0) is never valid.

## Decisions already made (do not reopen without a reason)

- Reservation state machine: `Pending → Confirmed` (payment completed), `Pending → Expired` (after `ExpirationDate`, 15 minutes after `ReservationDate`), `Confirmed → Cancelled` (user cancels after paying). Every method rejects a call from any other status with `InvalidReservationStatusException`.
- A declined payment does not cancel the reservation (the user can pay again until it expires). Cancelling releases the seat and refunds the payment.
- `Reservation.Create` rejects a reservation dated in the future or already expired. `Screening.ReserveSeat` validates its own rules (date set and UTC, before the screening starts, non-empty user id, seat exists).
- The background job runs every minute and loads only `Pending` reservations with `ExpirationDate` earlier than now.
- Out of scope: catalog and screening scheduling, user registration, notifications, ticket scanning, ticket annulment after a cancelled reservation.

## Open items

- What releases a seat when `ReserveSeat` succeeded but `Reservation.Create` failed for another reason.
- A payment that arrives after the reservation expired (`ConfirmReservation` throws, no path on the board).
- Race between the expiring job and a payment confirming the same reservation (handle in Application with optimistic concurrency): concurrency token (`rowversion`/`xmin`) configured in Infrastructure, `DbUpdateConcurrencyException` translated to an `AppException` in the repository, the job runs each `ExpireReservationCommand` in its own DI scope and skips conflicts.
- Not decided yet: how `Payment` allows another attempt after `PaymentDeclined`.
- Outbox + inbox: events are published after `UpdateAsync` without an outbox, so a failure after saving loses the event (e.g. `Expired` reservation whose seat is never released). Needs a unit of work (repositories must not call `SaveChanges` on their own). Inbox for idempotency, since `Seat.Release` throws on an already free seat.
- Not decided yet: whether `ConfirmReservation` takes `now` and rejects calls after `ExpirationDate`.
- Not decided yet: how the API returns the reservation id after `ReserveSeatCommand` (e.g. API generates `ReservationId` and passes it through the command and `SeatReservedEvent` to `Reservation.Create`).
- Remaining handlers: `ReservationCancelledEventHandler` (release seat), `ConfirmReservationCommandHandler`, `PaymentCompleted` integration event handler (with the Payments module).
- Infrastructure: repository implementations and registration, persistence, expiring background job (hosted service).
- `Reservation.Create`: extract the 15-minute expiration to a constant, like `CancellationWindow`.
