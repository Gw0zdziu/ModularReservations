using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class ScreeningDateNotUtcException(DateTimeKind kind)
    : DomainException($"Screening date must be a UTC date, but was {kind}.")
{
    public DateTimeKind Kind { get; } = kind;
}
