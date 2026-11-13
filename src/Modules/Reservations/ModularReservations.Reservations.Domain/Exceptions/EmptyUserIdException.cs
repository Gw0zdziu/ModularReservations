using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions;

public sealed class EmptyUserIdException()
    : DomainException("User id cannot be empty.");
