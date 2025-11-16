using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedersandadjustmentofPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_TripId",
                table: "Payments");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "Brand", "DailyPriceUsd", "Km", "LicensePlate", "Model", "Status", "YearOfManufacture" },
                values: new object[,]
                {
                    { 1, "Toyota", 50m, 90000, "AB 123 CD", "Corolla", 1, 2018 },
                    { 2, "Ford", 60m, 150000, "AC 124 DC", "Focus", 1, 2020 },
                    { 3, "Audi", 100m, 30000, "AB 125 CD", "A4", 1, 2019 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Dni", "Email", "Name", "Password", "PhoneNumber", "RoleId", "Surname" },
                values: new object[,]
                {
                    { 1, "11111111", "user@mail.com", "User", "user", "3411111111", 1, "User" },
                    { 2, "22222222", "admin@mail.com", "Admin", "admin", "342222222", 2, "Admin" },
                    { 3, "33333333", "superadmin@mail.com", "Super", "superadmin", "343333333", 3, "Admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TripId",
                table: "Payments",
                column: "TripId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_TripId",
                table: "Payments");

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AlterColumn<int>(
                name: "TotalAmount",
                table: "Payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TripId",
                table: "Payments",
                column: "TripId");
        }
    }
}
