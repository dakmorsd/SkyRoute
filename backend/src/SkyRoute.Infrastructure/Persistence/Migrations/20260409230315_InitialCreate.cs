using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyRoute.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "airports",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    CountryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_airports", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "providers",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_providers", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FlightNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OriginAirportCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    OriginCountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    DestinationAirportCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    DestinationCountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    DepartureTimeUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ArrivalTimeUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CabinClass = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RouteType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PassengerCount = table.Column<int>(type: "integer", nullable: false),
                    PerPassengerPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bookings_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "booking_passengers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_passengers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_booking_passengers_bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_booking_passengers_BookingId",
                table: "booking_passengers",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_ReferenceCode",
                table: "bookings",
                column: "ReferenceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bookings_UserId",
                table: "bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "airports");

            migrationBuilder.DropTable(
                name: "booking_passengers");

            migrationBuilder.DropTable(
                name: "providers");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
