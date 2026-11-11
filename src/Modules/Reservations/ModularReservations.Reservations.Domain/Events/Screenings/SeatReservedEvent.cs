using ModularReservations.Shared.Interfaces;

namespace ModularReservations.Reservations.Domain.Events.Screenings;

public record SeatReservedEvent(Guid ScreeningId, Guid SeatId, Guid UserId, DateTime ReservedAt) :  IEvent;
