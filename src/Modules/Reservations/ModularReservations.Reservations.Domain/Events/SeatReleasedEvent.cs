using ModularReservations.Shared.Interfaces;

namespace ModularReservations.Reservations.Domain.Events;

public record SeatReleasedEvent(Guid ScreeningId, Guid SeatId) : IEvent;
