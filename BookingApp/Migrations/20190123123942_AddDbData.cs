using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingApp.Migrations
{
    public partial class AddDbData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    RuleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    MinTime = table.Column<int>(nullable: false),
                    MaxTime = table.Column<int>(nullable: false),
                    StepTime = table.Column<int>(nullable: false),
                    ServiceTime = table.Column<int>(nullable: false),
                    ReuseTimeout = table.Column<int>(nullable: false),
                    PreOrderTimeLimit = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<Guid>(nullable: true),
                    CreatedByNavigationId = table.Column<string>(nullable: true),
                    UpdatedByNavigationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.RuleId);
                    table.ForeignKey(
                        name: "FK_Rules_AspNetUsers_CreatedByNavigationId",
                        column: x => x.CreatedByNavigationId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rules_AspNetUsers_UpdatedByNavigationId",
                        column: x => x.UpdatedByNavigationId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TreeGroups",
                columns: table => new
                {
                    TreeGroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    ParentTreeGroupId = table.Column<int>(nullable: true),
                    DefaultRuleId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<Guid>(nullable: true),
                    CreatedByNavigationId = table.Column<string>(nullable: true),
                    UpdatedByNavigationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeGroups", x => x.TreeGroupId);
                    table.ForeignKey(
                        name: "FK_TreeGroups_AspNetUsers_CreatedByNavigationId",
                        column: x => x.CreatedByNavigationId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeGroups_Rules_DefaultRuleId",
                        column: x => x.DefaultRuleId,
                        principalTable: "Rules",
                        principalColumn: "RuleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeGroups_TreeGroups_ParentTreeGroupId",
                        column: x => x.ParentTreeGroupId,
                        principalTable: "TreeGroups",
                        principalColumn: "TreeGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeGroups_AspNetUsers_UpdatedByNavigationId",
                        column: x => x.UpdatedByNavigationId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    ResourceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    TreeGroupId = table.Column<int>(nullable: true),
                    RuleId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<Guid>(nullable: true),
                    CreatedByNavigationId = table.Column<string>(nullable: true),
                    UpdatedByNavigationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.ResourceId);
                    table.ForeignKey(
                        name: "FK_Resources_AspNetUsers_CreatedByNavigationId",
                        column: x => x.CreatedByNavigationId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resources_Rules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "Rules",
                        principalColumn: "RuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resources_TreeGroups_TreeGroupId",
                        column: x => x.TreeGroupId,
                        principalTable: "TreeGroups",
                        principalColumn: "TreeGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resources_AspNetUsers_UpdatedByNavigationId",
                        column: x => x.UpdatedByNavigationId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ResourceId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    IsCancelled = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<Guid>(nullable: true),
                    CreatedByNavigationId = table.Column<string>(nullable: true),
                    UpdatedByNavigationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_CreatedByNavigationId",
                        column: x => x.CreatedByNavigationId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "ResourceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_UpdatedByNavigationId",
                        column: x => x.UpdatedByNavigationId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CreatedByNavigationId",
                table: "Bookings",
                column: "CreatedByNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ResourceId",
                table: "Bookings",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UpdatedByNavigationId",
                table: "Bookings",
                column: "UpdatedByNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_CreatedByNavigationId",
                table: "Resources",
                column: "CreatedByNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_RuleId",
                table: "Resources",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_TreeGroupId",
                table: "Resources",
                column: "TreeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_UpdatedByNavigationId",
                table: "Resources",
                column: "UpdatedByNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_CreatedByNavigationId",
                table: "Rules",
                column: "CreatedByNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_UpdatedByNavigationId",
                table: "Rules",
                column: "UpdatedByNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeGroups_CreatedByNavigationId",
                table: "TreeGroups",
                column: "CreatedByNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeGroups_DefaultRuleId",
                table: "TreeGroups",
                column: "DefaultRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeGroups_ParentTreeGroupId",
                table: "TreeGroups",
                column: "ParentTreeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeGroups_UpdatedByNavigationId",
                table: "TreeGroups",
                column: "UpdatedByNavigationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "TreeGroups");

            migrationBuilder.DropTable(
                name: "Rules");
        }
    }
}
