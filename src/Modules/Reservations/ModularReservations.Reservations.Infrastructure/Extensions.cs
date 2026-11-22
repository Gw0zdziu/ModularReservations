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
using ModularReservations.Reservations.Infrastructure.Jobs;
using ModularReservations.Reservations.Infrastructure.Persistence;
using ModularReservations.Reservations.Infrastructure.Repositories;
using ModularReservations.Shared.Interfaces.Command;
using ModularReservations.Shared.Interfaces.Event;

namespace ModularReservations.Reservations.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddReservations(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Reservations");
            /*?? throw new InvalidOperationException("Connection string 'Reservations' is missing.");*/
        services.AddDbContext<ReservationsDbContext.ReservationsDbContext>(options => options.UseSqlServer(connectionString,
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
        return services;
    }
}
