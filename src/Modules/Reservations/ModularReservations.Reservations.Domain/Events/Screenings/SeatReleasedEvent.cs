using ModularReservations.Shared.Interfaces;
using ModularReservations.Shared.Interfaces.Event;

namespace ModularReservations.Reservations.Domain.Events.Screenings;

public record SeatReleasedEvent(Guid ScreeningId, Guid SeatId) : IEvent;
