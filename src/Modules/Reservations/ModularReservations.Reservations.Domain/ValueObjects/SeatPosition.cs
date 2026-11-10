namespace ModularReservations.Reservations.Domain.ValueObjects;

public record SeatPosition
{
    public int RowNumber { get; }
    public int NumberInRow { get; }

    public  SeatPosition(int rowNumber, int numberInRow)
    {
        if (rowNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rowNumber));
        }

        if (numberInRow <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numberInRow));
        }
        RowNumber = rowNumber;
        NumberInRow = numberInRow;
    }
}
