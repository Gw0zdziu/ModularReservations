namespace ModularReservations.Reservations.Api.Dtos;

public record CancelReservationDto(Guid ReservationId, Guid UserId);
