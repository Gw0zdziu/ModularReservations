using Microsoft.AspNetCore.Mvc;
using ModularReservations.Reservations.Api.Dtos;
using ModularReservations.Reservations.Application.Commands.CancelReservation;
using ModularReservations.Reservations.Application.Commands.ReserveSeat;
using ModularReservations.Shared.Interfaces.Dispatcher;

namespace ModularReservations.Reservations.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public sealed class ReservationsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public ReservationsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> ReserveSeat([FromBody] ReserveSeatDto newReservation)
    {
        var reservationId = Guid.NewGuid();
        var reserveSeatCommand = new ReserveSeatCommand(newReservation.ScreeningId, newReservation.SeatId, newReservation.UserId, reservationId);
        await _dispatcher.SendAsync(reserveSeatCommand);
        return Ok(reservationId);
    }

    [HttpPut("cancel")]
    public async Task<ActionResult> CancelReservation([FromBody] CancelReservationDto reservation)
    {
        await _dispatcher.SendAsync(new CancelReservationCommand(reservation.ReservationId, reservation.UserId));
        return NoContent();
    }
}
