using ModularReservations.Shared.Interfaces.Command;

namespace ModularReservations.Reservations.Application.Commands.ReserveSeat;

public record ReserveSeatCommand(Guid ScreeningId, Guid SeatId, Guid UserId) : ICommand;
