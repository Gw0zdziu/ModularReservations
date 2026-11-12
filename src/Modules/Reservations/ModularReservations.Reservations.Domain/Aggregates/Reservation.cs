using ModularReservations.Reservations.Domain.Events.Reservations;
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
            throw new ArgumentNullException(nameof(reservationDate));
        }
        if (reservationDate.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Given reservation date must be UTC date");
        }
        if (reservationDate > now)
        {
            throw new ArgumentException("Reservation date cannot be in the future");
        }
        if (screeningId == Guid.Empty)
        {
            throw new ArgumentException("Screening id cannot be empty");
        }

        if (seatId == Guid.Empty)
        {
            throw new ArgumentException("Seat id cannot be empty");
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty");
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
            throw new ArgumentException("Reservation status must be Pending");
        }
        if (now > ExpirationDate)
        {
            ReservationStatus = ReservationStatus.Expired;
            AddEvent(new ReservationExpiredEvent(ScreeningId, SeatId, Id));
        }
    }

    public void ConfirmReservation(DateTime now)
    {
        if (ReservationStatus != ReservationStatus.Pending)
        {
            throw new ArgumentException("Reservation status must be Pending");
        }
        ReservationStatus = ReservationStatus.Confirmed;
        AddEvent(new ReservationConfirmedEvent(ScreeningId, SeatId, Id));
    }

    public  void CancelReservation()
    {
        if (ReservationStatus == ReservationStatus.Pending)
        {
            ReservationStatus = ReservationStatus.Cancelled;
            AddEvent(new ReservationCancelledEvent(ScreeningId, SeatId, Id));
        }
    }
}
