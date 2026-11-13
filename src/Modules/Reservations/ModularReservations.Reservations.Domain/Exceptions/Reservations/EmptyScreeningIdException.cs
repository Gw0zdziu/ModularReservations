using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Reservations;

public sealed class EmptyScreeningIdException()
    : DomainException("Screening id cannot be empty.");
