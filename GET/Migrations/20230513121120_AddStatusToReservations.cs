using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GET.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Reservations");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Reservations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Reservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reservations");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Reservations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Reservations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
