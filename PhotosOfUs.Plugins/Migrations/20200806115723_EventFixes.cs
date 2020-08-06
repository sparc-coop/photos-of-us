using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotosOfUs.Plugins.Migrations
{
    public partial class EventFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_User_UserID",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Event",
                newName: "PhotographerId");

            migrationBuilder.RenameColumn(
                name: "EventID",
                table: "Event",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Event_UserID",
                table: "Event",
                newName: "IX_Event_PhotographerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_User_PhotographerId",
                table: "Event",
                column: "PhotographerId",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_User_PhotographerId",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "PhotographerId",
                table: "Event",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Event",
                newName: "EventID");

            migrationBuilder.RenameIndex(
                name: "IX_Event_PhotographerId",
                table: "Event",
                newName: "IX_Event_UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_User_UserID",
                table: "Event",
                column: "UserID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
