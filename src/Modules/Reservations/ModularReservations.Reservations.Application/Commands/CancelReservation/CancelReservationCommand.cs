using ModularReservations.Shared.Interfaces.Command;

namespace ModularReservations.Reservations.Application.Commands.CancelReservation;

public record CancelReservationCommand(Guid ReservationId, Guid UserId) : ICommand;
