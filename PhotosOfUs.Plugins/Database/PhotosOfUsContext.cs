using Microsoft.EntityFrameworkCore;

namespace PhotosOfUs.Model.Models
{
    public partial class PhotosOfUsContext : DbContext
    {
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<User> Users { get; set; }
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
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");
                entity.Property(e => e.OrderDate).HasColumnType("datetime");
                entity.Property(e => e.Total).HasColumnType("decimal(19, 4)");

                entity.OwnsMany(e => e.OrderDetail, x =>
                {
                    x.HasKey(y => y.Id);
                    x.Property(y => y.UnitPrice).HasColumnType("decimal(19, 4)");
                });
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AzureId).HasColumnName("AzureID");
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.OwnsMany(e => e.UserIdentities, x =>
                {
                    x.HasKey(y => y.AzureID);
                    x.Property(y => y.AzureID)
                        .ValueGeneratedNever();

                    x.Property(y => y.CreateDate).HasColumnType("datetime");
                    x.Property(y => y.LastLoginDate).HasColumnType("datetime");
                });

                entity.OwnsMany(e => e.SocialMedia, x =>
                {
                    x.HasKey(y => y.Id);
                    x.Property(y => y.AzureId).HasColumnName("AzureID");
                    x.Property(y => y.Type).HasColumnName("Type");
                });

                entity.OwnsMany(e => e.Folders, x =>
                {
                    x.HasKey(y => y.Id);
                    x.Property(y => y.CreatedDate).HasColumnType("datetime");
                });
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event");
                entity.Property(e => e.EventId).HasColumnName("EventID");
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.OwnsMany(e => e.Cards, card =>
                {
                    card.HasKey(x => x.Id);
                    card.Property(x => x.CreatedDate).HasColumnType("datetime");
                    
                });
                entity.OwnsMany(x => x.Photos, photo =>
                {
                    photo.ToTable("Photo");
                    photo.HasKey(x => x.Id);
                    photo.Property(e => e.Price).HasColumnType("decimal(19, 4)");
                    photo.Property(e => e.UploadDate).HasColumnType("datetime");
                    photo.Ignore(x => x.FolderName);
                    photo.Ignore(x => x.Stream);
                    photo.Ignore(x => x.FileSize);
                    photo.Ignore(x => x.Resolution);
                    photo.Ignore(x => x.ThumbnailUrl);
                    photo.Ignore(x => x.WaterMarkUrl);

                    photo.OwnsMany(e => e.PhotoTag, x =>
                    {
                        x.HasKey(y => new { y.PhotoId, y.TagId });
                        x.Property(y => y.RegisterDate).HasColumnType("datetime");
                    });
                });
            });

            modelBuilder.Entity<PrintType>();

            modelBuilder.Entity<PrintPrice>();

            modelBuilder.Entity<Tag>();
        }
    }
}
