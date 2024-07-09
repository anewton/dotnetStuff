﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Sqlite.InitialCreate
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Catalogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Objects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CatalogId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MessierNumber = table.Column<string>(type: "TEXT", nullable: true),
                    NewGeneralCalatog = table.Column<string>(type: "TEXT", nullable: true),
                    ObjectType = table.Column<string>(type: "TEXT", nullable: true),
                    Constellation = table.Column<string>(type: "TEXT", nullable: true),
                    RightAscension = table.Column<string>(type: "TEXT", nullable: true),
                    Declination = table.Column<string>(type: "TEXT", nullable: true),
                    Magnitude = table.Column<decimal>(type: "TEXT", nullable: false),
                    DistanceLightYears = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Objects_Catalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Objects_CatalogId",
                table: "Objects",
                column: "CatalogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Objects");

            migrationBuilder.DropTable(
                name: "Catalogs");
        }
    }
}