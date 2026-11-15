using ModularReservations.Reservations.Domain.Exceptions.Screenings;
using ModularReservations.Reservations.Domain.ValueObjects;

namespace ModularReservations.Reservations.Domain.Tests.ValueObjects;

public class SeatPositionTests
{

    [Fact]
    public void Constructor_RowNumberEqualZero_ThrowsInvalidSeatRowNumberException()
    {
        var result = () => new SeatPosition(0, 1);
        Assert.Throws<InvalidSeatRowNumberException>(result);
    }

    [Fact]
    public void Constructor_RowNumberLessThanZero_ThrowsInvalidSeatRowNumberException()
    {
        var result = () => new SeatPosition(-1, 1);
        Assert.Throws<InvalidSeatRowNumberException>(result);
    }

    [Fact]
    public void Constructor_NumberInRowEqualZero_ThrowsInvalidSeatRowNumberException()
    {
        var result = () => new SeatPosition(2, 0);
        Assert.Throws<InvalidSeatNumberInRowException>(result);
    }

    [Fact]
    public void Constructor_NumberInRowLessThanZero_ThrowsInvalidSeatNumberInRowException()
    {
        var result = () => new SeatPosition(2, -5);
        Assert.Throws<InvalidSeatNumberInRowException>(result);
    }
}
