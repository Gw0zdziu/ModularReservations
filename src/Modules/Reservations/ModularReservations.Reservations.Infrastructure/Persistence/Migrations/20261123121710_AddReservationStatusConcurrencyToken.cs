using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularReservations.Reservations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationStatusConcurrencyToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReservationStatus",
                schema: "reservations",
                table: "Reservations",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "reservations",
                table: "Reservations",
                newName: "ReservationStatus");
        }
    }
}
