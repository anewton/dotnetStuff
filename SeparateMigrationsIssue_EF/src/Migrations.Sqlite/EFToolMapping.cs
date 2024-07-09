using Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrations.Sqlite;

[Obsolete("DO NOT USE: Only used by EF migrations tool to find the DbContext")]
[DbContext(typeof(DomainDataContext))]
public class EFToolMapping : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) { }
}
