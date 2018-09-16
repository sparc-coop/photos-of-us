using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace PhotosOfUs.Model.Models
{
    public partial class PhotosOfUsContext : DbContext
    {
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCart { get; set; }

        public PhotosOfUsContext(DbContextOptions<PhotosOfUsContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SocialMedia>(entity =>
            {
                entity.Property(e => e.Type).HasColumnName("AzureID");
                entity.Property(e => e.Type).HasColumnName("Type");
            });

            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasIndex(e => e.PhotographerId);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderDate).HasColumnType("datetime");
                entity.Property(e => e.Total).HasColumnType("decimal(19, 4)");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(19, 4)");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(19, 4)");
                entity.Property(e => e.UploadDate).HasColumnType("datetime");
                entity.Property(x => x.SuggestedTags)
                    //.HasConversion(x => JsonConvert.DeserializeObject<RootObject>(x),
                    //x => JsonConvert.SerializeObject(x))
                ;
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AzureId).HasColumnName("AzureID");
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
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

            modelBuilder.Ignore<RootObject>();
        }
    }
}
