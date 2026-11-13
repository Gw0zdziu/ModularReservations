using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions;

public sealed class ReservationDateNotUtcException(DateTimeKind kind)
    : DomainException($"Reservation date must be a UTC date, but was {kind}.")
{
    public DateTimeKind Kind { get; } = kind;
}
