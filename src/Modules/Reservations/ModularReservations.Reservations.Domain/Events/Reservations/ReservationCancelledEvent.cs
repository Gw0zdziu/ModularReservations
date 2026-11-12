using ModularReservations.Shared.Interfaces;

namespace ModularReservations.Reservations.Domain.Events.Reservations;

public record ReservationCancelledEvent(Guid ScreeningId, Guid SeatId, Guid ReservationId) : IEvent;
