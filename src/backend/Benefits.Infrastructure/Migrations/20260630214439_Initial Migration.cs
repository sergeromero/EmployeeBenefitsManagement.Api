using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Benefits.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BenefitTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenefitTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeNumber = table.Column<string>(type: "nchar(8)", fixedLength: true, maxLength: 8, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BenefitPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BenefitTypeId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenefitPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BenefitPlans_BenefitTypes_BenefitTypeId",
                        column: x => x.BenefitTypeId,
                        principalTable: "BenefitTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BenefitPlans_EnrollmentCategories_EnrollmentCategoryId",
                        column: x => x.EnrollmentCategoryId,
                        principalTable: "EnrollmentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeEnrollments",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BenefitPlanId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeEnrollments", x => new { x.EmployeeId, x.BenefitPlanId });
                    table.ForeignKey(
                        name: "FK_EmployeeEnrollments_BenefitPlans_BenefitPlanId",
                        column: x => x.BenefitPlanId,
                        principalTable: "BenefitPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeEnrollments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BenefitTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Health" },
                    { 2, "Vision" },
                    { 3, "Dental" },
                    { 4, "Life Insurance" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Finance" },
                    { 2, "Engineering" },
                    { 3, "Marketing" },
                    { 4, "Human Resources" },
                    { 5, "Sales" }
                });

            migrationBuilder.InsertData(
                table: "EnrollmentCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Health Core" },
                    { 2, "Dental Core" },
                    { 3, "Orthodontics" },
                    { 4, "Vision" },
                    { 5, "Life Insurance" }
                });

            migrationBuilder.InsertData(
                table: "BenefitPlans",
                columns: new[] { "Id", "BenefitTypeId", "Description", "EnrollmentCategoryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Basic medical coverage.", 1, "Health Basic" },
                    { 2, 1, "Enhanced medical coverage with lower deductibles.", 1, "Health Premium" },
                    { 3, 1, "Medical coverage for employee and dependents.", 1, "Health Family" },
                    { 4, 3, "Preventive and restorative dental care.", 2, "Dental Basic" },
                    { 5, 3, "Extended dental coverage including major procedures.", 2, "Dental Premium" },
                    { 6, 3, "Orthodontic treatment coverage for eligible dependents.", 3, "Orthodontic Coverage" },
                    { 7, 2, "Annual eye exams and prescription lenses.", 4, "Vision Basic" },
                    { 8, 2, "Enhanced vision benefits with higher reimbursement limits.", 4, "Vision Premium" },
                    { 9, 4, "Employer-paid life insurance coverage.", 5, "Life Insurance Basic" },
                    { 10, 4, "Optional supplemental life insurance.", 5, "Life Insurance Enhanced" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BenefitPlans_BenefitTypeId",
                table: "BenefitPlans",
                column: "BenefitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BenefitPlans_EnrollmentCategoryId",
                table: "BenefitPlans",
                column: "EnrollmentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEnrollments_BenefitPlanId",
                table: "EmployeeEnrollments",
                column: "BenefitPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeNumber",
                table: "Employees",
                column: "EmployeeNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeEnrollments");

            migrationBuilder.DropTable(
                name: "BenefitPlans");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "BenefitTypes");

            migrationBuilder.DropTable(
                name: "EnrollmentCategories");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
