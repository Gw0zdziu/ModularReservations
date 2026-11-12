using ModularReservations.Shared.Interfaces;

namespace ModularReservations.Reservations.Domain.Events.Reservations;

public record ReservationConfirmedEvent(Guid ScreeningId, Guid SeatId, Guid ReservationId) : IEvent
{

}
