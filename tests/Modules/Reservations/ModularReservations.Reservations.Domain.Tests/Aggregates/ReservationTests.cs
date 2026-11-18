using ModularReservations.Reservations.Domain.Aggregates;
using ModularReservations.Reservations.Domain.Exceptions;
using ModularReservations.Reservations.Domain.Exceptions.Reservations;
using ModularReservations.Reservations.Domain.Statuses;


namespace ModularReservations.Reservations.Domain.Tests.Aggregates;

public class ReservationTests
{
    #region Create method

    [Fact]
    public void Create_DefaultReservationDate_ThrowsReservationDateRequiredException()
    {
        var reservationDate = default(DateTime);
        var now = DateTime.Now;
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = () => Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        Assert.Throws<ReservationDateRequiredException>(result);
    }

    [Fact]
    public void Create_OtherKindReservationDate_ThrowsReservationDateNotUtcException()
    {
        var reservationDate = DateTime.Now;
        var now = DateTime.Now;
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = () => Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        Assert.Throws<ReservationDateNotUtcException>(result);
    }

    [Fact]
    public void Create_ReservationDateGreaterThanNow_ThrowsReservationDateInFutureException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddDays(1);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = () => Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        Assert.Throws<ReservationDateInFutureException>(result);
    }

    [Fact]
    public void Create_ReservationDateWith15MinutesLessThanNow_ThrowsReservationAlreadyExpiredException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-16);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = () => Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        Assert.Throws<ReservationAlreadyExpiredException>(result);
    }

    [Fact]
    public void Create_ScreeningIdIsEmpty_ThrowsEmptyScreeningIdException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningId = Guid.Empty;
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = () => Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        Assert.Throws<EmptyScreeningIdException>(result);
    }

    [Fact]
    public void Create_SeatIdIsEmpty_ThrowsEmptySeatIdException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.Empty;
        var userId = Guid.NewGuid();
        var result = () => Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        Assert.Throws<EmptySeatIdException>(result);
    }

    [Fact]
    public void Create_UserIdIsEmpty_ThrowsEmptyUserIdException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.Empty;
        var result = () => Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        Assert.Throws<EmptyUserIdException>(result);
    }

    [Fact]
    public void Create_ValidParameters_CreatesReservation()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        Assert.Equal(result.ScreeningId, screeningId);
        Assert.Equal(ReservationStatus.Pending, result.ReservationStatus);
        Assert.Equal(result.ExpirationDate, result.ReservationDate.AddMinutes(15));
    }

    #endregion

    #region ExpireReservation method

    [Fact]
    public void ExpireReservation_ReservationStatusOtherThanPending_ThrowsInvalidReservationStatusException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        var expireReservationNowDateTime = now.AddHours(1);
        result.ConfirmReservation();
        var exception = () => result.ExpireReservation(expireReservationNowDateTime);
        Assert.Throws<InvalidReservationStatusException>(exception);
    }

    [Fact]
    public void ExpireReservation_SuccessfullyExpireReservation_ChangeReservationStatusOnExpired()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        var expireReservationNowDateTime = now.AddHours(1);
        result.ExpireReservation(expireReservationNowDateTime);
        Assert.Equal(ReservationStatus.Expired, result.ReservationStatus);
    }

    #endregion

    #region ConfirmReservation

    [Fact]
    public void ConfirmReservation_ReservationStatusOtherThanPending_ThrowsInvalidReservationStatusException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        var expireReservationNowDateTime = now.AddHours(1);
        result.ExpireReservation(expireReservationNowDateTime);
        var exception = result.ConfirmReservation;
        Assert.Throws<InvalidReservationStatusException>(exception);
    }

    [Fact]
    public void ConfirmReservation_SuccessfullyConfirmReservation_ChangeReservationStatusOnConfirmed()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        result.ConfirmReservation();
        Assert.Equal(ReservationStatus.Confirmed, result.ReservationStatus);
    }

    #endregion

    #region CancelReservation

    [Fact]
    public void CancelReservation_ReservationStatusOtherThanConfirmed_ThrowsInvalidReservationStatusException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningDate = DateTime.UtcNow;
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        var expireReservationNowDateTime = now.AddHours(1);
        result.ExpireReservation(expireReservationNowDateTime);
        var exception = () => result.CancelReservation(userId,screeningDate, now);
        Assert.Throws<InvalidReservationStatusException>(exception);
    }

    [Fact]
    public void CancelReservation_NowDateGreaterThanScreeningDate_ThrowsScreeningAlreadyStartedException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningDate = now.AddHours(-1);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        result.ConfirmReservation();
        var exception = () => result.CancelReservation(userId,screeningDate, now);
        Assert.Throws<ScreeningAlreadyStartedException>(exception);
    }

    [Fact]
    public void CancelReservation_NowAfterCancellationDeadline_ThrowsCancellationDeadlinePassedException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningDate = now.AddMinutes(10);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        result.ConfirmReservation();
        var exception = () => result.CancelReservation(userId, screeningDate, now);
        Assert.Throws<CancellationDeadlinePassedException>(exception);
    }

    [Fact]
    public void CancelReservation_NowEqualsCancellationDeadline_ThrowsCancellationDeadlinePassedException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningDate = now.AddMinutes(30);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        result.ConfirmReservation();
        var exception = () => result.CancelReservation(userId, screeningDate, now);
        Assert.Throws<CancellationDeadlinePassedException>(exception);
    }

    [Fact]
    public void CancelReservation_ReservationNotOwnedByUserId_ThrowsReservationNotOwnedByUserException()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningDate = now.AddHours(1);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var result = Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        result.ConfirmReservation();
        var exception = () => result.CancelReservation(anotherUserId, screeningDate, now);
        Assert.Throws<ReservationNotOwnedByUserException>(exception);
    }

    [Fact]
    public void CancelReservation_SuccessfullyConfirmReservation_ChangeReservationStatusOnCancelled()
    {
        var now = DateTime.UtcNow;
        var reservationDate = now.AddMinutes(-10);
        var screeningDate = now.AddHours(1);
        var screeningId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = Reservation.Create(now, screeningId, reservationDate, seatId, userId);
        result.ConfirmReservation();
        result.CancelReservation(userId, screeningDate, now);
        Assert.Equal(ReservationStatus.Cancelled, result.ReservationStatus);
    }

    #endregion
}
