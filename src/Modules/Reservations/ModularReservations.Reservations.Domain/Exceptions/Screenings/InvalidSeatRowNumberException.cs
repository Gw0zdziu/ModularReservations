using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class InvalidSeatRowNumberException(int rowNumber)
    : DomainException($"Seat row number must be greater than 0, but was {rowNumber}.")
{
    public int RowNumber { get; } = rowNumber;
}
