using ModularReservations.Shared.Interfaces;
using ModularReservations.Shared.Interfaces.Event;

namespace ModularReservations.Reservations.Domain.Events.Reservations;

public record ReservationCancelledEvent(Guid ScreeningId, Guid SeatId, Guid ReservationId) : IEvent;
