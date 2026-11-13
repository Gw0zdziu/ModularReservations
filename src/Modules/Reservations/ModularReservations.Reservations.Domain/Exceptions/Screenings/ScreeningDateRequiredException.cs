using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class ScreeningDateRequiredException()
    : DomainException("Screening date must be specified.");
