using ModularReservations.Reservations.Domain.Entities;
using ModularReservations.Reservations.Domain.ValueObjects;

namespace ModularReservations.Reservations.Domain.Aggregates;

public class Screening
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
            throw new ArgumentNullException(nameof(screeningDate));
        }
        if (screeningDate.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Given screening date must be UTC date");
        }

        if (screeningDate <= now)
        {
            throw new ArgumentException("Given screening date must be in the future");
        }
        if (movieId == Guid.Empty)
        {
            throw new ArgumentException("Movie id must be specified");
        }
        if (seatPositions is null )
        {
            throw new ArgumentNullException(nameof(seatPositions));
        }
        if (seatPositions.Count == 0)
        {
            throw new ArgumentException("No seats have been added");
        }
        var uniqueSeats = new HashSet<SeatPosition>();
        foreach (var seatPosition in seatPositions)
        {
            if (seatPosition is null)
            {
                throw new ArgumentException("No seats have been added");
            }
            var isExist = uniqueSeats.Add(seatPosition);
            if (!isExist)
            {
                throw new ArgumentException($"Seat {seatPosition.NumberInRow} in row: {seatPosition.RowNumber} already exists");
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
}
