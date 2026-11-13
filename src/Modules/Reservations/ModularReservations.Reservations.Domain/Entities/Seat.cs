using ModularReservations.Reservations.Domain.Exceptions.Screenings;
using ModularReservations.Reservations.Domain.ValueObjects;

namespace ModularReservations.Reservations.Domain.Entities;

public class Seat
{
    public Guid Id { get; private set; }
    public SeatPosition Position { get; private set; }

    public bool IsReserved { get; private set; }

    private Seat()
    {

    }

    internal static Seat Create(SeatPosition position)
    {
        if (position == null)
        {
            throw new SeatPositionRequiredException();
        }
        return new Seat
        {
            Id = Guid.NewGuid(),
            Position = position,
            IsReserved = false
        };
    }

    internal void Reserve()
    {
        if (IsReserved)
        {
            throw new SeatAlreadyReservedException(Id, Position.RowNumber, Position.NumberInRow);
        };
        IsReserved = true;
    }

    internal void Release()
    {
        if (!IsReserved)
        {
            throw new SeatNotReservedException(Id, Position.RowNumber, Position.NumberInRow);
        }
        IsReserved = false;
    }
}
