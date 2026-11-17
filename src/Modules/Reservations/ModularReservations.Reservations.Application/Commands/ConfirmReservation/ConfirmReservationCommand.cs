using ModularReservations.Shared.Interfaces.Command;

namespace ModularReservations.Reservations.Application.Commands.ConfirmReservation;

public record ConfirmReservationCommand(Guid ReservationId) : ICommand;
