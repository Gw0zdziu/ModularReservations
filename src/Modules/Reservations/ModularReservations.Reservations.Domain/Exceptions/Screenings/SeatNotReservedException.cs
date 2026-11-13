using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class SeatNotReservedException(Guid seatId, int rowNumber, int numberInRow)
    : DomainException($"The seat {numberInRow} in row {rowNumber} is not reserved, so it cannot be released.")
{
    public Guid SeatId { get; } = seatId;
    public int RowNumber { get; } = rowNumber;
    public int NumberInRow { get; } = numberInRow;
}
