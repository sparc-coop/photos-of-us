using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotosOfUs.Core.Events;
using PhotosOfUs.Core.Orders;
using PhotosOfUs.Core.Photos;
using PhotosOfUs.Core.Users;
using System;

namespace PhotosOfUs.Model.Models
{
    public partial class PhotosOfUsContext : DbContext
    {
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Event> Events { get; set; }

        public PhotosOfUsContext()
        {

        }

        public PhotosOfUsContext(DbContextOptions<PhotosOfUsContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                // TODO: Figure out a way to remove the connection string below. This is being used by the EF Migrations
                //throw new Exception("Make sure you've selected the correct DB on the PhotosOfUsContext class");
                string conn = "Server=photosofus.database.windows.net;Database=photosofus-dev;User Id=kuviocreative;Password=;";

                builder.UseSqlServer(conn);
            }

            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(ConfigureOrder);

            modelBuilder.Entity<User>(ConfigureUser);

            modelBuilder.Entity<Photo>(ConfigurePhoto);

            modelBuilder.Entity<Event>(ConfigureEvent);

            modelBuilder.Entity<PrintType>();

            modelBuilder.Entity<PrintPrice>();

            modelBuilder.Entity<Tag>();

            modelBuilder.Entity<Folder>(ConfigureFolder);
        }

        private void ConfigurePhoto(EntityTypeBuilder<Photo> entity)
        {
            entity.ToTable("Photo");
            entity.HasKey(x => x.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.UploadDateUtc).HasColumnType("datetime");
            entity.Ignore(x => x.FolderName);
            entity.Ignore(x => x.Stream);
            entity.Ignore(x => x.FileSize);
            entity.Ignore(x => x.Resolution);
            entity.Ignore(x => x.ThumbnailUrl);
            entity.Ignore(x => x.WaterMarkUrl);

            entity.HasOne(x => x.Photographer)
                .WithMany()
                .HasForeignKey(x => x.PhotographerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Event)
                .WithMany(x => x.Photos)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.OwnsMany(e => e.PhotoTag, x =>
            {
                x.HasKey(y => new { y.PhotoId, y.TagId });
                x.Property(y => y.RegisterDateUtc).HasColumnType("datetime");
            });
        }

        private void ConfigureFolder(EntityTypeBuilder<Folder> entity)
        {
            entity.ToTable("Folder");
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Photographer)
                .WithMany()
                .HasForeignKey(x => x.PhotographerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureOrder(EntityTypeBuilder<Order> entity)
        {
            entity.ToTable("Order");
            entity.Property(e => e.OrderDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Total).HasColumnType("decimal(19, 4)");

            entity.OwnsMany(e => e.OrderDetails, x =>
            {
                x.HasKey(y => y.Id);
                x.Property(y => y.UnitPrice).HasColumnType("decimal(19, 4)");
                x.OnDelete(DeleteBehavior.Restrict);
            });

            entity.HasOne(x => x.ShippingAddress)
                .WithMany()
                .HasForeignKey(x => x.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.BillingAddress)
                .WithMany()
                .HasForeignKey(x => x.BillingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.Status)
            .IsRequired()
            .HasConversion(
                g => g.Value,
                y => OrderStatus.Get(y));
        }

        private void ConfigureEvent(EntityTypeBuilder<Event> entity)
        {
            entity.ToTable("Event");
            entity.HasKey(e => e.Id);
            entity.HasOne(x => x.Photographer)
                .WithMany()
                .HasForeignKey(x => x.PhotographerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.OwnsMany(e => e.Cards, card =>
            {
                card.HasKey(x => x.Id);
                card.Property(x => x.CreatedDateUtc).HasColumnType("datetime");

            });
            //entity.HasMany(x => x.Photos, photo =>
            //{
            //    photo.HasKey(x => x.Id);
            //});
        }

        private void ConfigureUser(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("User");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AzureId).HasColumnName("AzureID");
            entity.Property(e => e.CreateDateUtc).HasColumnType("datetime");

            entity.OwnsMany(e => e.UserIdentities, x =>
            {
                x.HasKey(y => y.AzureID);
                x.Property(y => y.AzureID)
                    .ValueGeneratedNever();

                x.Property(y => y.CreateDateUtc).HasColumnType("datetime");
                x.Property(y => y.LastLoginDateUtc).HasColumnType("datetime");
            });

            //entity.OwnsMany(e => e.SocialMedia, x =>
            //{
            //    x.HasKey(y => y.Id);
            //    x.Property(y => y.AzureId).HasColumnName("AzureID");
            //    x.Property(y => y.Type).HasColumnName("Type");
            //});

            //entity.OwnsMany(e => e.Folders, x =>
            //{
            //    x.HasKey(y => y.Id);
            //    x.Property(y => y.CreatedDateUtc).HasColumnType("datetime");
            //});

            entity.Property(x => x.Role)
            .IsRequired()
            .HasConversion(
                g => g.Value,
                y => Role.Get(y));
        }

        

    }
}
