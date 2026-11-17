using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Application.Exceptions;

public class ReservationNotFoundException(Guid reservationId) : AppException($"Reservation {reservationId} not found")
{
    public Guid ReservationId { get; init; } = reservationId;
}
