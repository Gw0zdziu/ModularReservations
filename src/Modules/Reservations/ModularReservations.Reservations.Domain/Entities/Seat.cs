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
            throw new ArgumentNullException(nameof(position));
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
            throw new InvalidOperationException($"The seat {Position.NumberInRow} in row {Position.RowNumber} is already reserved.");
        };
        IsReserved = true;
    }

    internal void Release()
    {
        if (!IsReserved)
        {
            throw new InvalidOperationException($"The seat {Position.NumberInRow} in row {Position.RowNumber} is already released.");
        }
        IsReserved = false;
    }
}
