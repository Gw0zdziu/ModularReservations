using ModularReservations.Shared.Interfaces;

namespace ModularReservations.Reservations.Domain.Events.Screenings;

public record SeatReleasedEvent(Guid ScreeningId, Guid SeatId) : IEvent;
