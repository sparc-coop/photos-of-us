using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Migrations
{
    public partial class PublicProfilePhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Card_Photographer",
                table: "Card");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PhotosOfUs.Model.Models.User_TempId",
                table: "PhotosOfUs.Model.Models.User");

            migrationBuilder.DropColumn(
                name: "TempId",
                table: "PhotosOfUs.Model.Models.User");

            migrationBuilder.RenameTable(
                name: "PhotosOfUs.Model.Models.User",
                newName: "User");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "User",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "AzureID",
                table: "User",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "User",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "User",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "User",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "User",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPhotographer",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "User",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "User",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address1 = table.Column<string>(unicode: false, maxLength: 250, nullable: false),
                    Address2 = table.Column<string>(unicode: false, maxLength: 250, nullable: false),
                    City = table.Column<string>(unicode: false, maxLength: 125, nullable: false),
                    Country = table.Column<string>(unicode: false, maxLength: 3, nullable: false),
                    Email = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    FullName = table.Column<string>(unicode: false, maxLength: 250, nullable: false),
                    Phone = table.Column<string>(unicode: false, maxLength: 25, nullable: false),
                    State = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    ZipCode = table.Column<string>(unicode: false, maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Folder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Name = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    PhotographerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folder_Photographer",
                        column: x => x.PhotographerId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrintType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Height = table.Column<string>(maxLength: 20, nullable: true),
                    Icon = table.Column<string>(maxLength: 20, nullable: true),
                    Length = table.Column<string>(maxLength: 20, nullable: true),
                    Type = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserIdentity",
                columns: table => new
                {
                    AzureID = table.Column<string>(maxLength: 64, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IdentityProvider = table.Column<string>(nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentity", x => x.AzureID);
                    table.ForeignKey(
                        name: "FK_UserIdentity_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BillingAddressId = table.Column<int>(nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    OrderStatus = table.Column<string>(nullable: true),
                    ShippingAddressId = table.Column<int>(nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19, 4)", nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Photo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(unicode: false, maxLength: 30, nullable: false),
                    FolderId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(unicode: false, maxLength: 200, nullable: false),
                    PhotographerId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(19, 4)", nullable: true),
                    PublicProfile = table.Column<bool>(nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Url = table.Column<string>(unicode: false, maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photo_Folder",
                        column: x => x.FolderId,
                        principalTable: "Folder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Photo_Photographer",
                        column: x => x.PhotographerId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrintPrice",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PhotographerId = table.Column<int>(nullable: false),
                    Price = table.Column<string>(maxLength: 10, nullable: true),
                    PrintId = table.Column<int>(nullable: false),
                    PrintTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintPrice", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PrintPrice_User_PhotographerId",
                        column: x => x.PhotographerId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintPrice_PrintType_PrintTypeId",
                        column: x => x.PrintTypeId,
                        principalTable: "PrintType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    PhotoId = table.Column<int>(nullable: false),
                    PrintTypeId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(19, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Photo",
                        column: x => x.PhotoId,
                        principalTable: "Photo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDetail_PrintType",
                        column: x => x.PrintTypeId,
                        principalTable: "PrintType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CartCode = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    PhotoId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Photo_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserId",
                table: "Address",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Folder_PhotographerId",
                table: "Folder",
                column: "PhotographerId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_BillingAddressId",
                table: "Order",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ShippingAddressId",
                table: "Order",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                table: "Order",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderId",
                table: "OrderDetail",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_PhotoId",
                table: "OrderDetail",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_PrintTypeId",
                table: "OrderDetail",
                column: "PrintTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Photo_FolderId",
                table: "Photo",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Photo_PhotographerId",
                table: "Photo",
                column: "PhotographerId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintPrice_PhotographerId",
                table: "PrintPrice",
                column: "PhotographerId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintPrice_PrintTypeId",
                table: "PrintPrice",
                column: "PrintTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_PhotoId",
                table: "ShoppingCart",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentity_UserID",
                table: "UserIdentity",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Card_Photographer",
                table: "Card",
                column: "PhotographerId",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Card_Photographer",
                table: "Card");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "PrintPrice");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.DropTable(
                name: "UserIdentity");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "PrintType");

            migrationBuilder.DropTable(
                name: "Photo");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Folder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "User");

            migrationBuilder.DropColumn(
                name: "AzureID",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsPhotographer",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "PhotosOfUs.Model.Models.User");

            migrationBuilder.AddColumn<int>(
                name: "TempId",
                table: "PhotosOfUs.Model.Models.User",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_PhotosOfUs.Model.Models.User_TempId",
                table: "PhotosOfUs.Model.Models.User",
                column: "TempId");

            migrationBuilder.AddForeignKey(
                name: "FK_Card_Photographer",
                table: "Card",
                column: "PhotographerId",
                principalTable: "PhotosOfUs.Model.Models.User",
                principalColumn: "TempId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
