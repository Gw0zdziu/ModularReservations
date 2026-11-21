namespace ModularReservations.Reservations.Api.Dtos;

public record ReserveSeatDto(Guid ScreeningId, Guid SeatId, Guid UserId);
