using ModularReservations.Shared.Interfaces;

namespace ModularReservations.Reservations.Domain.Events.Reservations;

public record ReservationExpiredEvent(Guid ScreeningId, Guid SeatId, Guid ReservationId) : IEvent;
