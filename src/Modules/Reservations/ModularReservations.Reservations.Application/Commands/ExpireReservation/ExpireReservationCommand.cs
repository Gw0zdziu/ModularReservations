using ModularReservations.Shared.Interfaces.Command;

namespace ModularReservations.Reservations.Application.Commands.ExpireReservation;

public record ExpireReservationCommand(Guid ReservationId) : ICommand;
