using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class ScreeningSeatsRequiredException()
    : DomainException("Screening must have at least one seat and no seat position can be null.");
