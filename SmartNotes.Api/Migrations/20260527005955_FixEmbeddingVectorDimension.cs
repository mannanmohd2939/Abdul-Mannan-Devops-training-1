using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace SmartNotes.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixEmbeddingVectorDimension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "Notes",
                type: "vector(1536)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "Notes",
                type: "vector(2)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(1536)");
        }
    }
}
