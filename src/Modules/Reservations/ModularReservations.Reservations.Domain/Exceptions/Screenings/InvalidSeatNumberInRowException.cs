using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class InvalidSeatNumberInRowException(int numberInRow)
    : DomainException($"Seat number in row must be greater than 0, but was {numberInRow}.")
{
    public int NumberInRow { get; } = numberInRow;
}
