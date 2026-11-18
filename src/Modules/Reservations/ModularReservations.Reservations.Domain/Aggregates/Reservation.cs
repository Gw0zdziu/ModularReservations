using ModularReservations.Reservations.Domain.Events.Reservations;
using ModularReservations.Reservations.Domain.Exceptions;
using ModularReservations.Reservations.Domain.Exceptions.Reservations;
using ModularReservations.Reservations.Domain.Statuses;
using ModularReservations.Shared.Aggregates;

namespace ModularReservations.Reservations.Domain.Aggregates;

public class Reservation : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ScreeningId { get; private set; }
    public Guid SeatId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime ReservationDate { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public ReservationStatus ReservationStatus { get; private set; }
    private static readonly TimeSpan CancellationWindow = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan ExpirationWindow = TimeSpan.FromMinutes(15);

    private  Reservation()
    {
    }

    public static Reservation Create(DateTime now, Guid screeningId, DateTime reservationDate, Guid seatId, Guid userId)
    {
        if (reservationDate == default)
        {
            throw new ReservationDateRequiredException();
        }
        if (reservationDate.Kind != DateTimeKind.Utc)
        {
            throw new ReservationDateNotUtcException(reservationDate.Kind);
        }
        if (reservationDate > now)
        {
            throw new ReservationDateInFutureException(reservationDate, now);
        }

        var expirationDeadline = reservationDate + ExpirationWindow;
        if (expirationDeadline < now)
        {
            throw new ReservationAlreadyExpiredException(expirationDeadline);
        }
        if (screeningId == Guid.Empty)
        {
            throw new EmptyScreeningIdException();
        }

        if (seatId == Guid.Empty)
        {
            throw new EmptySeatIdException();
        }

        if (userId == Guid.Empty)
        {
            throw new EmptyUserIdException();
        }
        return new Reservation
        {
            Id = Guid.NewGuid(),
            ScreeningId =  screeningId,
            SeatId =  seatId,
            UserId =  userId,
            ReservationDate = reservationDate,
            ExpirationDate =  expirationDeadline,
            ReservationStatus = ReservationStatus.Pending,

        };
    }

    public void ExpireReservation(DateTime now)
    {
        if (ReservationStatus != ReservationStatus.Pending)
        {
            throw new InvalidReservationStatusException(Id, ReservationStatus, ReservationStatus.Pending);
        }
        if (now < ExpirationDate)
        {
            throw new ReservationNotExpiredYetException(Id, ExpirationDate, now);
        }
        ReservationStatus = ReservationStatus.Expired;
        AddEvent(new ReservationExpiredEvent(ScreeningId, SeatId, Id));
    }

    public void ConfirmReservation()
    {
        if (ReservationStatus != ReservationStatus.Pending)
        {
            throw new InvalidReservationStatusException(Id, ReservationStatus, ReservationStatus.Pending);
        }
        ReservationStatus = ReservationStatus.Confirmed;
        AddEvent(new ReservationConfirmedEvent(ScreeningId, SeatId, Id));
    }

    public  void CancelReservation(Guid userId, DateTime screeningDate, DateTime now)
    {
        if (userId != UserId)
        {
            throw new ReservationNotOwnedByUserException(Id, userId);
        }
        if (ReservationStatus != ReservationStatus.Confirmed)
        {
            throw new InvalidReservationStatusException(Id, ReservationStatus, ReservationStatus.Confirmed);
        }
        if (now > screeningDate)
        {
            throw new ScreeningAlreadyStartedException(ScreeningId, screeningDate);
        }

        var cancellationDeadLine = screeningDate - CancellationWindow;
        if (now >= cancellationDeadLine)
        {
            throw new CancellationDeadlinePassedException(Id, cancellationDeadLine, now);
        }
        ReservationStatus = ReservationStatus.Cancelled;
        AddEvent(new ReservationCancelledEvent(ScreeningId, SeatId, Id));
    }
}
