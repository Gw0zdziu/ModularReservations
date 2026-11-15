using System.Collections.ObjectModel;
using ModularReservations.Reservations.Domain.Aggregates;
using ModularReservations.Reservations.Domain.Exceptions.Screenings;
using ModularReservations.Reservations.Domain.ValueObjects;

namespace ModularReservations.Reservations.Domain.Tests.Aggregates;

public class ScreeningTests
{
    #region Create method

    [Fact]
    public void Create_DefaultScreeningDate_ThrowsScreeningDateRequiredException()
    {
        var screeningDate = default(DateTime);
        var now = DateTime.UtcNow;
        var movieId = Guid.NewGuid();
        var seatPositionsList = new List<SeatPosition>()
        {
            new(1, 1),
            new(1, 2),
            new(1, 3),
            new(1, 4),
        };
        var seatPositions = new ReadOnlyCollection<SeatPosition>(seatPositionsList);
        var result = () => Screening.Create(screeningDate, now, movieId, seatPositions);
        Assert.Throws<ScreeningDateRequiredException>(result);

    }

    [Fact]
    public void Create_OtherDateTimeKindThanUtc_ThrowsScreeningDateNotUtcException()
    {
        var screeningDate = DateTime.Now;
        var now = DateTime.UtcNow;
        var movieId = Guid.NewGuid();
        var seatPositionsList = new List<SeatPosition>()
        {
            new(1, 1),
            new(1, 2),
            new(1, 3),
            new(1, 4),
        };
        var seatPositions = new ReadOnlyCollection<SeatPosition>(seatPositionsList);
        var result = () => Screening.Create(screeningDate, now, movieId, seatPositions);
        Assert.Throws<ScreeningDateNotUtcException>(result);
    }

    [Fact]
    public void Create_ScreeningDateLessOrEqualNowDate_ThrowsScreeningDateNotInFutureException()
    {
        var now = DateTime.UtcNow;
        var screeningDate = now.AddHours(-1);
        var movieId = Guid.NewGuid();
        var seatPositionsList = new List<SeatPosition>()
        {
            new(1, 1),
            new(1, 2),
            new(1, 3),
            new(1, 4),
        };
        var seatPositions = new ReadOnlyCollection<SeatPosition>(seatPositionsList);
        var result = () => Screening.Create(screeningDate, now, movieId, seatPositions);
        Assert.Throws<ScreeningDateNotInFutureException>(result);
    }

    [Fact]
    public void Create_MovieIdIsEmpty_ThrowsEmptyMovieIdException()
    {
        var now = DateTime.UtcNow;
        var screeningDate = now.AddHours(1);
        var movieId = Guid.Empty;
        var seatPositionsList = new List<SeatPosition>()
        {
            new(1, 1),
            new(1, 2),
            new(1, 3),
            new(1, 4),
        };
        var seatPositions = new ReadOnlyCollection<SeatPosition>(seatPositionsList);
        var result = () => Screening.Create(screeningDate, now, movieId, seatPositions);
        Assert.Throws<EmptyMovieIdException>(result);
    }

    [Fact]
    public void Create_SeatPositionsIsNull_ThrowsScreeningSeatsRequiredException()
    {
        var now = DateTime.UtcNow;
        var screeningDate = now.AddHours(1);
        var movieId = Guid.NewGuid();
        var result = () => Screening.Create(screeningDate, now, movieId, null);
        Assert.Throws<ScreeningSeatsRequiredException>(result);
    }

    [Fact]
    public void Create_SeatPositionsIsEmpty_ThrowsScreeningSeatsRequiredException()
    {
        var now = DateTime.UtcNow;
        var screeningDate = now.AddHours(1);
        var movieId = Guid.NewGuid();
        var seatPositionsList = new List<SeatPosition>();
        var seatPositions = new ReadOnlyCollection<SeatPosition>(seatPositionsList);
        var result = () => Screening.Create(screeningDate, now, movieId, seatPositions);
        Assert.Throws<ScreeningSeatsRequiredException>(result);
    }

    [Fact]
    public void Create_AtLeastOneSeatPositionIsNull_ThrowsScreeningSeatsRequiredException()
    {
        var now = DateTime.UtcNow;
        var screeningDate = now.AddHours(1);
        var movieId = Guid.NewGuid();
        var seatPositionsList = new List<SeatPosition?>()
        {
            new(1, 1),
            new(1, 2),
            null,
            new(1, 3),

        };
        var seatPositions = new ReadOnlyCollection<SeatPosition>(seatPositionsList!);
        var result = () => Screening.Create(screeningDate, now, movieId, seatPositions);
        Assert.Throws<ScreeningSeatsRequiredException>(result);
    }

    [Fact]
    public void Create_SeatPositionExist_ThrowsDuplicateSeatPositionException()
    {
        var now = DateTime.UtcNow;
        var screeningDate = now.AddHours(1);
        var movieId = Guid.NewGuid();
        var seatPositionsList = new List<SeatPosition>()
        {
            new(1, 1),
            new(1, 2),
            new(1, 3),
            new(1, 2),

        };
        var seatPositions = new ReadOnlyCollection<SeatPosition>(seatPositionsList);
        var result = () => Screening.Create(screeningDate, now, movieId, seatPositions);
        Assert.Throws<DuplicateSeatPositionException>(result);
    }

    [Fact]
    public void Create_ValidParameters_CreateScreening()
    {
        var now = DateTime.UtcNow;
        var screeningDate = now.AddHours(1);
        var movieId = Guid.NewGuid();
        var seatPositionsList = new List<SeatPosition>()
        {
            new(1, 1),
            new(1, 2),
            new(1, 3),
            new(1, 4),

        };
        var seatPositions = new ReadOnlyCollection<SeatPosition>(seatPositionsList);
        var result = Screening.Create(screeningDate, now, movieId, seatPositions);
        Assert.Equal(4, result.Seats.Count);
    }

    #endregion
}
