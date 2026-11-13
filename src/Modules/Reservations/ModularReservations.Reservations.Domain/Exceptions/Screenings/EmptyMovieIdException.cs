using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class EmptyMovieIdException()
    : DomainException("Movie id must be specified.");
