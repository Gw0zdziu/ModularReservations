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
        if (reservationDate.AddMinutes(15) < now)
        {
            throw new ReservationAlreadyExpiredException(reservationDate.AddMinutes(15));
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
            ExpirationDate =  reservationDate.AddMinutes(15),
            ReservationStatus = ReservationStatus.Pending,

        };
    }

    public void ExpireReservation(DateTime now)
    {
        if (ReservationStatus != ReservationStatus.Pending)
        {
            throw new InvalidReservationStatusException(Id, ReservationStatus, ReservationStatus.Pending);
        }
        if (now > ExpirationDate)
        {
            ReservationStatus = ReservationStatus.Expired;
            AddEvent(new ReservationExpiredEvent(ScreeningId, SeatId, Id));
        }
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

    public  void CancelReservation()
    {
        if (ReservationStatus != ReservationStatus.Confirmed)
        {
            throw new InvalidReservationStatusException(Id, ReservationStatus, ReservationStatus.Confirmed);
        }
        ReservationStatus = ReservationStatus.Cancelled;
        AddEvent(new ReservationCancelledEvent(ScreeningId, SeatId, Id));
    }
}
