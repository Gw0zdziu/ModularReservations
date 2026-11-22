using ModularReservations.Reservations.Api.Controllers;
using ModularReservations.Reservations.Infrastructure;
using ModularReservations.Shared.Infrastructure.Dispatcher;
using ModularReservations.Shared.Interfaces.Dispatcher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddOpenApi();
builder.Services.AddReservations(builder.Configuration);
builder.Services.AddScoped<IDispatcher, Dispatcher>();
builder.Services.AddControllers()
    .AddApplicationPart(typeof(ReservationsController).Assembly);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
