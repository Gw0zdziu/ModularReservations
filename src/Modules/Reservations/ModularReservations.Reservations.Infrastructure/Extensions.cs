using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularReservations.Reservations.Application.Commands.CancelReservation;
using ModularReservations.Reservations.Application.Commands.ConfirmReservation;
using ModularReservations.Reservations.Application.Commands.ExpireReservation;
using ModularReservations.Reservations.Application.Commands.ReserveSeat;
using ModularReservations.Reservations.Application.EventHandlers.ReservationCancelled;
using ModularReservations.Reservations.Application.EventHandlers.ReservationExpired;
using ModularReservations.Reservations.Application.EventHandlers.SeatReserved;
using ModularReservations.Reservations.Application.Interfaces;
using ModularReservations.Reservations.Domain.Events.Reservations;
using ModularReservations.Reservations.Domain.Events.Screenings;
using ModularReservations.Reservations.Domain.Interfaces.Repositories;
using ModularReservations.Reservations.Infrastructure.DbContextReservations;
using ModularReservations.Reservations.Infrastructure.Jobs;
using ModularReservations.Reservations.Infrastructure.Persistence;
using ModularReservations.Reservations.Infrastructure.Repositories;
using ModularReservations.Shared.Infrastructure.Messaging;
using ModularReservations.Shared.Interfaces.Command;
using ModularReservations.Shared.Interfaces.Event;

namespace ModularReservations.Reservations.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddReservations(this IServiceCollection services, IConfiguration configuration)
    {

        var connectionString = configuration.GetConnectionString("Reservations")
            ?? throw new InvalidOperationException("Connection string 'Reservations' is missing.");
        services.AddDbContext<ReservationsDbContext>(options => options.UseSqlServer(connectionString,
            sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "reservations")));
        services.AddScoped<IEventHandler<SeatReservedEvent>, SeatReservedEventHandler>();
        services.AddScoped<IEventHandler<ReservationExpiredEvent>, ReservationExpiredEventHandler>();
        services.AddScoped<IEventHandler<ReservationCancelledEvent>, ReservationCancelledEventHandler>();
        services.AddScoped<ICommandHandler<ConfirmReservationCommand>, ConfirmReservationCommandHandler>();
        services.AddScoped<ICommandHandler<ReserveSeatCommand>, ReserveSeatCommandHandler>();
        services.AddScoped<ICommandHandler<ExpireReservationCommand>, ExpireReservationCommandHandler>();
        services.AddScoped<ICommandHandler<CancelReservationCommand>, CancelReservationCommandHandler>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IScreeningRepository, ScreeningRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddHostedService<ExpireReservationJob>();
        services.Configure<EventTypeOptions>(options =>
        {
            options.Register<SeatReservedEvent>("reservations.seat_reserved");
            options.Register<SeatReleasedEvent>("reservations.seat_released");
            options.Register<ReservationExpiredEvent>("reservations.reservation_expired");
            options.Register<ReservationCancelledEvent>("reservations.reservation_cancelled");
            options.Register<ReservationConfirmedEvent>("reservations.reservation_confirmed");
        });
        return services;
    }
}
