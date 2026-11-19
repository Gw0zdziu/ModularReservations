using ModularReservations.Reservations.Infrastructure;
using ModularReservations.Shared.Infrastructure.Dispatcher;
using ModularReservations.Shared.Interfaces.Dispatcher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddReservations(builder.Configuration);
builder.Services.AddScoped<IDispatcher, Dispatcher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
