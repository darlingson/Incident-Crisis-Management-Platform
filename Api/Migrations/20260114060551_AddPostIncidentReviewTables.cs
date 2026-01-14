using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPostIncidentReviewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostIncidentReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReportId = table.Column<int>(type: "integer", nullable: false),
                    RootCause = table.Column<string>(type: "text", nullable: false),
                    DetectionMethod = table.Column<string>(type: "text", nullable: false),
                    ReviewedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostIncidentReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostIncidentReviews_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContributingFactors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostIncidentReviewId = table.Column<int>(type: "integer", nullable: false),
                    Factor = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributingFactors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContributingFactors_PostIncidentReviews_PostIncidentReviewId",
                        column: x => x.PostIncidentReviewId,
                        principalTable: "PostIncidentReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreventiveActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostIncidentReviewId = table.Column<int>(type: "integer", nullable: false),
                    ActionItem = table.Column<string>(type: "text", nullable: false),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreventiveActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreventiveActions_PostIncidentReviews_PostIncidentReviewId",
                        column: x => x.PostIncidentReviewId,
                        principalTable: "PostIncidentReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecoverySteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostIncidentReviewId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoverySteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecoverySteps_PostIncidentReviews_PostIncidentReviewId",
                        column: x => x.PostIncidentReviewId,
                        principalTable: "PostIncidentReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContributingFactors_PostIncidentReviewId",
                table: "ContributingFactors",
                column: "PostIncidentReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_PostIncidentReviews_ReportId",
                table: "PostIncidentReviews",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_PreventiveActions_PostIncidentReviewId",
                table: "PreventiveActions",
                column: "PostIncidentReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_RecoverySteps_PostIncidentReviewId",
                table: "RecoverySteps",
                column: "PostIncidentReviewId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContributingFactors");

            migrationBuilder.DropTable(
                name: "PreventiveActions");

            migrationBuilder.DropTable(
                name: "RecoverySteps");

            migrationBuilder.DropTable(
                name: "PostIncidentReviews");
        }
    }
}
