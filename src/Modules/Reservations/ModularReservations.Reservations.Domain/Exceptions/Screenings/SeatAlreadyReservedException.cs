using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class SeatAlreadyReservedException(Guid seatId, int rowNumber, int numberInRow)
    : DomainException($"The seat {numberInRow} in row {rowNumber} is already reserved.")
{
    public Guid SeatId { get; } = seatId;
    public int RowNumber { get; } = rowNumber;
    public int NumberInRow { get; } = numberInRow;
}
