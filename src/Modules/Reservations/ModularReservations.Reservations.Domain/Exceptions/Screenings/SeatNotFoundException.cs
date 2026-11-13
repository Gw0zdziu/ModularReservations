using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class SeatNotFoundException(Guid screeningId, Guid seatId)
    : DomainException($"Seat {seatId} does not exist in the screening {screeningId}.")
{
    public Guid ScreeningId { get; } = screeningId;
    public Guid SeatId { get; } = seatId;
}
