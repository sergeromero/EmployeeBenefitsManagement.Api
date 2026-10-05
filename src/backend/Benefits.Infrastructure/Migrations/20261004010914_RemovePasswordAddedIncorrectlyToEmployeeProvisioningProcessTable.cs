using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Benefits.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePasswordAddedIncorrectlyToEmployeeProvisioningProcessTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "ProvisioningProcesses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "ProvisioningProcesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
