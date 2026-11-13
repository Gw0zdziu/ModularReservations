using ModularReservations.Reservations.Domain.Entities;
using ModularReservations.Reservations.Domain.Events;
using ModularReservations.Reservations.Domain.Events.Screenings;
using ModularReservations.Reservations.Domain.Exceptions;
using ModularReservations.Reservations.Domain.Exceptions.Screenings;
using ModularReservations.Reservations.Domain.ValueObjects;
using ModularReservations.Shared.Aggregates;

namespace ModularReservations.Reservations.Domain.Aggregates;

public class Screening : AggregateRoot
{
    private List<Seat> _seats = [];
    public Guid Id { get; private set; }
    public DateTime ScreeningDate { get; private set; }
    public Guid MovieId { get; private set; }

    public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();

    private Screening()
    {

    }
    public static Screening Create(DateTime screeningDate, DateTime now, Guid movieId, IReadOnlyCollection<SeatPosition> seatPositions)
    {
        if (screeningDate == default)
        {
            throw new ScreeningDateRequiredException();
        }
        if (screeningDate.Kind != DateTimeKind.Utc)
        {
            throw new ScreeningDateNotUtcException(screeningDate.Kind);
        }

        if (screeningDate <= now)
        {
            throw new ScreeningDateNotInFutureException(screeningDate, now);
        }
        if (movieId == Guid.Empty)
        {
            throw new EmptyMovieIdException();
        }
        if (seatPositions is null || seatPositions.Count == 0)
        {
            throw new ScreeningSeatsRequiredException();
        }
        var uniqueSeats = new HashSet<SeatPosition>();
        foreach (var seatPosition in seatPositions)
        {
            if (seatPosition is null)
            {
                throw new ScreeningSeatsRequiredException();
            }
            var isExist = uniqueSeats.Add(seatPosition);
            if (!isExist)
            {
                throw new DuplicateSeatPositionException(seatPosition.RowNumber, seatPosition.NumberInRow);
            }
        }

        var seats = new List<Seat>();
        foreach (var seatPosition in seatPositions)
        {
            var seat = Seat.Create(seatPosition);
            seats.Add(seat);
        }

        return new Screening
        {
            Id =  Guid.NewGuid(),
            ScreeningDate = screeningDate,
            MovieId = movieId,
            _seats = seats
        };

    }

    public void ReserveSeat(Guid seatId, Guid userId, DateTime reservationDate)
    {
        if (reservationDate == default)
        {
            throw new ReservationDateRequiredException();
        }
        if (reservationDate.Kind != DateTimeKind.Utc)
        {
            throw new ReservationDateNotUtcException(reservationDate.Kind);
        }
        if (reservationDate >= ScreeningDate)
        {
            throw new ScreeningAlreadyStartedException(Id, ScreeningDate);
        }
        if (userId == Guid.Empty)
        {
            throw new EmptyUserIdException();
        }
        var seat = _seats.FirstOrDefault(s => s.Id == seatId);
        if (seat is null)
        {
            throw new SeatNotFoundException(Id, seatId);
        }
        seat.Reserve();
        AddEvent(new SeatReservedEvent(Id, seatId, userId, reservationDate));
    }

    public void ReleaseSeat(Guid seatId)
    {
        var seat = _seats.FirstOrDefault(s => s.Id == seatId);
        if (seat is null)
        {
            throw new SeatNotFoundException(Id, seatId);
        }
        seat.Release();
        AddEvent(new SeatReleasedEvent(Id, seatId));
    }


}
