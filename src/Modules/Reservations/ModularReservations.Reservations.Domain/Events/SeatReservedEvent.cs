using ModularReservations.Reservations.Domain.Entities;
using ModularReservations.Shared.Interfaces;

namespace ModularReservations.Reservations.Domain.Events;

public record SeatReservedEvent(Guid ScreeningId, Guid SeatId, Guid UserId, DateTime ReservedAt) :  IEvent;
