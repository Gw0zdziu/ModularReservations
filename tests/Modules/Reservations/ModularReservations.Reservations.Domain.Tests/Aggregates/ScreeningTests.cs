using System.Collections.ObjectModel;
using ModularReservations.Reservations.Domain.Aggregates;
using ModularReservations.Reservations.Domain.Exceptions;
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
    public void Create_ScreeningDateLessThanNowDate_ThrowsScreeningDateNotInFutureException()
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
    public void Create_ScreeningDateEqualNowDate_ThrowsScreeningDateNotInFutureException()
    {
        var now = DateTime.UtcNow;
        var screeningDate = now;
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

    #region ReserveSeat

    [Fact]
    public void ReserveSeat_DefaultReservationDate_ThrowsReservationDateRequiredException()
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
        var reservation = Screening.Create(screeningDate, now, movieId, seatPositions);
        var reservationDate = default(DateTime);
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = () => reservation.ReserveSeat(seatId, userId, reservationDate);
        Assert.Throws<ReservationDateRequiredException>(result);
    }

    [Fact]
    public void ReserveSeat_ReservationDateOtherThanUtc_ThrowsReservationDateNotUtcException()
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
        var reservation = Screening.Create(screeningDate, now, movieId, seatPositions);
        var reservationDate = DateTime.Now;
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = () => reservation.ReserveSeat(seatId, userId, reservationDate);
        Assert.Throws<ReservationDateNotUtcException>(result);
    }

    [Fact]
    public void ReserveSeat_ReservationDateGreaterThanScreeningDate_ThrowsScreeningAlreadyStartedException()
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
        var reservation = Screening.Create(screeningDate, now, movieId, seatPositions);
        var reservationDate = screeningDate.AddMinutes(30);
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = () => reservation.ReserveSeat(seatId, userId, reservationDate);
        Assert.Throws<ScreeningAlreadyStartedException>(result);
    }

    [Fact]
    public void ReserveSeat_UserIdEmpty_ThrowsEmptyUserIdException()
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
        var reservation = Screening.Create(screeningDate, now, movieId, seatPositions);
        var reservationDate = screeningDate.AddHours(-1);
        var seatId = Guid.NewGuid();
        var userId = Guid.Empty;
        var result = () => reservation.ReserveSeat(seatId, userId, reservationDate);
        Assert.Throws<EmptyUserIdException>(result);
    }

    [Fact]
    public void ReserveSeat_SeatNotFound_ThrowsSeatNotFoundException()
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
        var reservation = Screening.Create(screeningDate, now, movieId, seatPositions);
        var reservationDate = screeningDate.AddHours(-1);
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = () => reservation.ReserveSeat(seatId, userId, reservationDate);
        Assert.Throws<SeatNotFoundException>(result);
    }

    [Fact]
    public void ReserveSeat_ValidParameters_SeatReserved()
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
        var reservation = Screening.Create(screeningDate, now, movieId, seatPositions);
        var reservationDate = screeningDate.AddHours(-1);
        var seatId = reservation.Seats.First().Id;
        var userId = Guid.NewGuid();
        reservation.ReserveSeat(seatId, userId, reservationDate);
        Assert.True(reservation.Seats.First().IsReserved);
    }

    [Fact]
    public void ReserveSeat_ReserveTwoTimesOnOneSeat_ThrowsSeatAlreadyReservedException()
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
        var reservation = Screening.Create(screeningDate, now, movieId, seatPositions);
        var reservationDate = screeningDate.AddHours(-1);
        var seatId = reservation.Seats.First().Id;
        var firstUserId = Guid.NewGuid();
        var secondUserId = Guid.NewGuid();
        reservation.ReserveSeat(seatId, firstUserId, reservationDate);
        var result = () => reservation.ReserveSeat(seatId, secondUserId, reservationDate);
        Assert.Throws<SeatAlreadyReservedException>(result);
    }

    #endregion

    #region ReleaseSeat

    [Fact]
    public void ReleaseSeat_SeatNotFound_ThrowsSeatNotFoundException()
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
        var reservation = Screening.Create(screeningDate, now, movieId, seatPositions);
        var seatId = Guid.NewGuid();
        var result = () => reservation.ReleaseSeat(seatId);
        Assert.Throws<SeatNotFoundException>(result);
    }

    [Fact]
    public void ReleaseSeat_ReleasedUnReservedSeat_ThrowsSeatNotReservedException()
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
        var reservation = Screening.Create(screeningDate, now, movieId, seatPositions);
        var seatId = reservation.Seats.First().Id;
        var result = () => reservation.ReleaseSeat(seatId);
        Assert.Throws<SeatNotReservedException>(result);
    }

    [Fact]
    public void ReleaseSeat_SeatFound_ReleaseSeat()
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
        var reservation = Screening.Create(screeningDate, now, movieId, seatPositions);
        var seatId = reservation.Seats.First().Id;
        var reservationDate = screeningDate.AddHours(-1);
        var userId = Guid.NewGuid();
        reservation.ReserveSeat(seatId, userId, reservationDate);
        reservation.ReleaseSeat(seatId);
        Assert.False(reservation.Seats.First().IsReserved);
    }



    #endregion
}
