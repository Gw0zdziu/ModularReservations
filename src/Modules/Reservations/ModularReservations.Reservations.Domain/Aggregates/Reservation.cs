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
    public ReservationStatus Status { get; private set; }
    private static readonly TimeSpan CancellationWindow = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan ExpirationWindow = TimeSpan.FromMinutes(15);

    private  Reservation()
    {
    }

    public static Reservation Create(Guid reservationId,DateTime now, Guid screeningId, DateTime reservationDate, Guid seatId, Guid userId)
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
        if (reservationId == Guid.Empty)
        {
            throw new EmptyReservationIdException();
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
            Id = reservationId,
            ScreeningId =  screeningId,
            SeatId =  seatId,
            UserId =  userId,
            ReservationDate = reservationDate,
            ExpirationDate =  expirationDeadline,
            Status = ReservationStatus.Pending,

        };
    }

    public void ExpireReservation(DateTime now)
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new InvalidReservationStatusException(Id, Status, ReservationStatus.Pending);
        }
        if (now < ExpirationDate)
        {
            throw new ReservationNotExpiredYetException(Id, ExpirationDate, now);
        }
        Status = ReservationStatus.Expired;
        AddEvent(new ReservationExpiredEvent(ScreeningId, SeatId, Id));
    }

    public void ConfirmReservation()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new InvalidReservationStatusException(Id, Status, ReservationStatus.Pending);
        }
        Status = ReservationStatus.Confirmed;
        AddEvent(new ReservationConfirmedEvent(ScreeningId, SeatId, Id));
    }

    public  void CancelReservation(Guid userId, DateTime screeningDate, DateTime now)
    {
        if (userId != UserId)
        {
            throw new ReservationNotOwnedByUserException(Id, userId);
        }
        if (Status != ReservationStatus.Confirmed)
        {
            throw new InvalidReservationStatusException(Id, Status, ReservationStatus.Confirmed);
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
        Status = ReservationStatus.Cancelled;
        AddEvent(new ReservationCancelledEvent(ScreeningId, SeatId, Id));
    }
}
