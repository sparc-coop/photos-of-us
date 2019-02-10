using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Claims;
using Kuvio.Kernel.Auth;

namespace PhotosOfUs.Model.Models
{
    public partial class PhotosOfUsContext : DbContext
    {
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Event> Events { get; set; }

        public PhotosOfUsContext(DbContextOptions<PhotosOfUsContext> options) : base(options)
        { 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SocialMedia>(entity =>
            {
                entity.Property(e => e.AzureId).HasColumnName("AzureID");
                entity.Property(e => e.Type).HasColumnName("Type");
            });

            modelBuilder.Entity<Card>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");
                entity.Property(e => e.OrderDate).HasColumnType("datetime");
                entity.Property(e => e.Total).HasColumnType("decimal(19, 4)");
                //entity.Ignore(x => x.Amount);
                //entity.Ignore(x => x.TotalPaid);
                //entity.Ignore(x => x.Earning);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(19, 4)");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.ToTable("Photo");
                entity.Property(e => e.Price).HasColumnType("decimal(19, 4)");
                entity.Property(e => e.UploadDate).HasColumnType("datetime");
                entity.Ignore(x => x.FolderName);
                entity.Ignore(x => x.Stream);
                entity.Ignore(x => x.FileSize);
                entity.Ignore(x => x.Resolution);
                entity.Ignore(x => x.ThumbnailUrl);
                entity.Ignore(x => x.WaterMarkUrl);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AzureId).HasColumnName("AzureID");
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event");
                entity.Property(e => e.EventId).HasColumnName("EventID");
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserIdentity>(entity =>
            {
                entity.HasKey(e => e.AzureID);
                entity.Property(e => e.AzureID)
                    .ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<PrintType>();

            modelBuilder.Entity<PrintPrice>();

            modelBuilder.Entity<Tag>();

            modelBuilder.Entity<PhotoTag>(entity =>
            {
                entity.HasKey(x => new { x.PhotoId, x.TagId });
                entity.Property(e => e.RegisterDate).HasColumnType("datetime");
            });
        }
    }
}
