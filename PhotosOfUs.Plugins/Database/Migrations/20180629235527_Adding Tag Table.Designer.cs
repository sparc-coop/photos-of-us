﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using PhotosOfUs.Model.Models;
using System;

namespace PhotosOfUs.Model.Migrations
{
    [DbContext(typeof(PhotosOfUsContext))]
    [Migration("20180629235527_Adding Tag Table")]
    partial class AddingTagTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PhotosOfUs.Model.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address1")
                        .IsRequired()
                        .HasMaxLength(250)
                        .IsUnicode(false);

                    b.Property<string>("Address2")
                        .IsRequired()
                        .HasMaxLength(250)
                        .IsUnicode(false);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(125)
                        .IsUnicode(false);

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(3)
                        .IsUnicode(false);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .IsUnicode(false);

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(25)
                        .IsUnicode(false);

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<int>("UserId");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasMaxLength(25)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Address");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<int>("PhotographerId");

                    b.HasKey("Id");

                    b.HasIndex("PhotographerId");

                    b.ToTable("Card");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.Folder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<int>("PhotographerId");

                    b.HasKey("Id");

                    b.HasIndex("PhotographerId");

                    b.ToTable("Folder");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BillingAddressId");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime");

                    b.Property<string>("OrderStatus");

                    b.Property<int>("ShippingAddressId");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(19, 4)");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("BillingAddressId");

                    b.HasIndex("ShippingAddressId");

                    b.HasIndex("UserId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.OrderDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("OrderId");

                    b.Property<int>("PhotoId");

                    b.Property<int>("PrintTypeId");

                    b.Property<int>("Quantity");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(19, 4)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("PhotoId");

                    b.HasIndex("PrintTypeId");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false);

                    b.Property<int>("FolderId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .IsUnicode(false);

                    b.Property<int>("PhotographerId");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(19, 4)");

                    b.Property<bool>("PublicProfile");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("FolderId");

                    b.HasIndex("PhotographerId");

                    b.ToTable("Photo");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.PrintPrice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID");

                    b.Property<int>("PhotographerId")
                        .HasColumnName("PhotographerId");

                    b.Property<string>("Price")
                        .HasColumnName("Price")
                        .HasMaxLength(10);

                    b.Property<int>("PrintId")
                        .HasColumnName("PrintId");

                    b.Property<int?>("PrintTypeId");

                    b.HasKey("Id");

                    b.HasIndex("PhotographerId");

                    b.HasIndex("PrintTypeId");

                    b.ToTable("PrintPrice");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.PrintType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<string>("Height")
                        .HasColumnName("Height")
                        .HasMaxLength(20);

                    b.Property<string>("Icon")
                        .HasColumnName("Icon")
                        .HasMaxLength(20);

                    b.Property<string>("Length")
                        .HasColumnName("Length")
                        .HasMaxLength(20);

                    b.Property<string>("Type")
                        .HasColumnName("Type")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("PrintType");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.ShoppingCartItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CartCode");

                    b.Property<DateTime>("DateCreated");

                    b.Property<int>("PhotoId");

                    b.Property<int>("Quantity");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("PhotoId");

                    b.ToTable("ShoppingCart");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.SocialMedia", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AzureId");

                    b.Property<string>("Link");

                    b.Property<string>("Type")
                        .HasColumnName("Type")
                        .HasMaxLength(128);

                    b.Property<int?>("UserId");

                    b.Property<string>("Username")
                        .HasColumnName("Username");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SocialMedia");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<string>("TagName")
                        .HasColumnName("TagName");

                    b.HasKey("Id");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID");

                    b.Property<string>("AzureId")
                        .HasColumnName("AzureID")
                        .HasMaxLength(128);

                    b.Property<string>("Bio")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(128);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Facebook");

                    b.Property<string>("FirstName")
                        .HasMaxLength(128);

                    b.Property<bool?>("IsPhotographer");

                    b.Property<string>("JobPosition")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("LastLoginDate")
                        .HasColumnType("datetime");

                    b.Property<string>("LastName")
                        .HasMaxLength(128);

                    b.Property<string>("ProfilePhotoUrl")
                        .HasMaxLength(1000)
                        .IsUnicode(false);

                    b.Property<int>("TemplateSelected");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.UserIdentity", b =>
                {
                    b.Property<string>("AzureID")
                        .HasColumnName("AzureID")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("IdentityProvider");

                    b.Property<DateTime?>("LastLoginDate")
                        .HasColumnType("datetime");

                    b.Property<int>("UserID")
                        .HasColumnName("UserID");

                    b.HasKey("AzureID");

                    b.HasIndex("UserID");

                    b.ToTable("UserIdentity");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.Address", b =>
                {
                    b.HasOne("PhotosOfUs.Model.Models.User")
                        .WithOne("Address")
                        .HasForeignKey("PhotosOfUs.Model.Models.Address", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.Card", b =>
                {
                    b.HasOne("PhotosOfUs.Model.Models.User", "Photographer")
                        .WithMany("Card")
                        .HasForeignKey("PhotographerId")
                        .HasConstraintName("FK_Card_Photographer")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.Folder", b =>
                {
                    b.HasOne("PhotosOfUs.Model.Models.User", "Photographer")
                        .WithMany("Folder")
                        .HasForeignKey("PhotographerId")
                        .HasConstraintName("FK_Folder_Photographer");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.Order", b =>
                {
                    b.HasOne("PhotosOfUs.Model.Models.Address", "BillingAddress")
                        .WithMany("OrderBillingAddress")
                        .HasForeignKey("BillingAddressId")
                        .HasConstraintName("FK_Order_BillingAddressId");

                    b.HasOne("PhotosOfUs.Model.Models.Address", "ShippingAddress")
                        .WithMany("OrderShippingAddress")
                        .HasForeignKey("ShippingAddressId")
                        .HasConstraintName("FK_Order_ShippingAddressId");

                    b.HasOne("PhotosOfUs.Model.Models.User", "User")
                        .WithMany("Order")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Order_User");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.OrderDetail", b =>
                {
                    b.HasOne("PhotosOfUs.Model.Models.Order", "Order")
                        .WithMany("OrderDetail")
                        .HasForeignKey("OrderId")
                        .HasConstraintName("FK_OrderDetail_Order");

                    b.HasOne("PhotosOfUs.Model.Models.Photo", "Photo")
                        .WithMany("OrderDetail")
                        .HasForeignKey("PhotoId")
                        .HasConstraintName("FK_OrderDetail_Photo");

                    b.HasOne("PhotosOfUs.Model.Models.PrintType", "PrintType")
                        .WithMany("OrderDetail")
                        .HasForeignKey("PrintTypeId")
                        .HasConstraintName("FK_OrderDetail_PrintType");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.Photo", b =>
                {
                    b.HasOne("PhotosOfUs.Model.Models.Folder", "Folder")
                        .WithMany("Photo")
                        .HasForeignKey("FolderId")
                        .HasConstraintName("FK_Photo_Folder");

                    b.HasOne("PhotosOfUs.Model.Models.User", "Photographer")
                        .WithMany("Photo")
                        .HasForeignKey("PhotographerId")
                        .HasConstraintName("FK_Photo_Photographer");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.PrintPrice", b =>
                {
                    b.HasOne("PhotosOfUs.Model.Models.User", "Photographer")
                        .WithMany("PrintPrice")
                        .HasForeignKey("PhotographerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PhotosOfUs.Model.Models.PrintType", "PrintType")
                        .WithMany()
                        .HasForeignKey("PrintTypeId");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.ShoppingCartItem", b =>
                {
                    b.HasOne("PhotosOfUs.Model.Models.Photo", "Photo")
                        .WithMany()
                        .HasForeignKey("PhotoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.SocialMedia", b =>
                {
                    b.HasOne("PhotosOfUs.Model.Models.User")
                        .WithMany("SocialMedia")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("PhotosOfUs.Model.Models.UserIdentity", b =>
                {
                    b.HasOne("PhotosOfUs.Model.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}