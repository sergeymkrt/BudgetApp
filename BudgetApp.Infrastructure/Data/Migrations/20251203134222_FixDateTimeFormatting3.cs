using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixDateTimeFormatting3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE ""Transactions""
SET ""Date"" = (""Date"" AT TIME ZONE 'UTC');

UPDATE ""Alerts""
SET ""CreatedAt"" = (""CreatedAt"" AT TIME ZONE 'UTC');
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
