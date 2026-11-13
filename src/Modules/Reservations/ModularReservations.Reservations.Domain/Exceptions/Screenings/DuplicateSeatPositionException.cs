using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class DuplicateSeatPositionException(int rowNumber, int numberInRow)
    : DomainException($"Seat {numberInRow} in row {rowNumber} already exists.")
{
    public int RowNumber { get; } = rowNumber;
    public int NumberInRow { get; } = numberInRow;
}
