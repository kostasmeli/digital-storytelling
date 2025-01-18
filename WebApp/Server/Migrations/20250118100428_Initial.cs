using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlazorApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DialogueObjects",
                columns: table => new
                {
                    Title = table.Column<string>(type: "text", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogueObjects", x => x.Title);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    CorrectAnswerId = table.Column<int>(type: "integer", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    DialogueObjectTitle = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_DialogueObjects_DialogueObjectTitle",
                        column: x => x.DialogueObjectTitle,
                        principalTable: "DialogueObjects",
                        principalColumn: "Title",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DialogueSessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    DialogueTitle = table.Column<string>(type: "text", nullable: false),
                    MaxScore = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogueSessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_DialogueSessions_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answer_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[,]
                {
                    { "Kwstasmel", "KonstantinosMel@gmail.com", "Konstantinos", "Melissourgos", "1q2w3e4r", "Admin" },
                    { "UserMeli", "KonstantinosMel@gmail.com", "Konstantinos", "Melissourgos", "test123", "User" }
                });

            migrationBuilder.InsertData(
                table: "DialogueSessions",
                columns: new[] { "id", "DialogueTitle", "MaxScore", "Score", "Username", "date" },
                values: new object[,]
                {
                    { new Guid("8ee0e5ea-a90a-4609-a599-cf5844a6b6e1"), "Test", 10, 1, "UserMeli", new DateTime(2025, 1, 18, 12, 4, 27, 531, DateTimeKind.Utc).AddTicks(4860) },
                    { new Guid("df03861f-a70c-4d2f-baa9-73bcb058ee3d"), "Test", 10, 1, "UserMeli", new DateTime(2025, 1, 18, 12, 4, 27, 541, DateTimeKind.Utc).AddTicks(120) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answer_QuestionId",
                table: "Answer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogueSessions_Username",
                table: "DialogueSessions",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_DialogueObjectTitle",
                table: "Questions",
                column: "DialogueObjectTitle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "DialogueSessions");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "DialogueObjects");
        }
    }
}
