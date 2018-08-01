using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace PhotosOfUs.Model.Models
{
    public partial class PhotosOfUsContext : DbContext
    {
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<SocialMedia> SocialMedia { get; set; }
        public virtual DbSet<Card> Card { get; set; }
        public virtual DbSet<Folder> Folder { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<Photo> Photo { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserIdentity> UserIdentity { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCart { get; set; }
        public virtual DbSet<PrintType> PrintType { get; set; }
        public virtual DbSet<PrintPrice> PrintPrice { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<PhotoTag> PhotoTag { get; set; }

        public PhotosOfUsContext(DbContextOptions<PhotosOfUsContext> options) : base(options)
        { }


        public PhotosOfUsContext()
        { }

        public IConfigurationRoot Configuration { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Trusted_Connection=False;Encrypt=True;");
                base.OnConfiguring(optionsBuilder);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.Address1)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Address2)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(125)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ZipCode)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserId);
            });

            modelBuilder.Entity<SocialMedia>(entity =>
            {
                entity.Property(e => e.Type)
                    .HasColumnName("AzureID")
                    .HasMaxLength(128);

                entity.Property(e => e.Type)
                    .HasColumnName("Type");

                entity.Property(e => e.Link);

                entity.Property(e => e.Username)
                    .HasColumnName("Username");
            });

            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasIndex(e => e.PhotographerId);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Photographer)
                    .WithMany(p => p.Card)
                    .HasForeignKey(d => d.PhotographerId)
                    .HasConstraintName("FK_Card_Photographer");
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Photographer)
                    .WithMany(p => p.Folder)
                    .HasForeignKey(d => d.PhotographerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Folder_Photographer");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.Total).HasColumnType("decimal(19, 4)");

                entity.HasOne(d => d.BillingAddress)
                    .WithMany(p => p.OrderBillingAddress)
                    .HasForeignKey(d => d.BillingAddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_BillingAddressId");

                entity.HasOne(d => d.ShippingAddress)
                    .WithMany(p => p.OrderShippingAddress)
                    .HasForeignKey(d => d.ShippingAddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_ShippingAddressId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(19, 4)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Order");

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.PhotoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Photo");

                entity.HasOne(d => d.PrintType)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.PrintTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_PrintType");

            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.UploadDate).HasColumnType("datetime");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.HasOne(d => d.Folder)
                    .WithMany(p => p.Photo)
                    .HasForeignKey(d => d.FolderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Photo_Folder");

                entity.HasOne(d => d.Photographer)
                    .WithMany(p => p.Photo)
                    .HasForeignKey(d => d.PhotographerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Photo_Photographer");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AzureId)
                    .HasColumnName("AzureID")
                    .HasMaxLength(128);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DisplayName).HasMaxLength(128);

                entity.Property(e => e.JobPosition).HasMaxLength(128);

                entity.Property(e => e.Bio).HasMaxLength(1000);

                entity.Property(e => e.ProfilePhotoUrl)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.FirstName).HasMaxLength(128);

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.LastName).HasMaxLength(128);

                entity.Property(e => e.IsPhotographer);
            });

            modelBuilder.Entity<UserIdentity>(entity =>
            {
                entity.HasKey(e => e.AzureID);

                entity.Property(e => e.AzureID)
                    .HasColumnName("AzureID")
                    .HasMaxLength(64)
                    .ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.IdentityProvider);

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.UserID).HasColumnName("UserID");
            });

            modelBuilder.Entity<PrintType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Type)
                    .HasColumnName("Type")
                    .HasMaxLength(50);

                entity.Property(e => e.Height)
                    .HasColumnName("Height")
                    .HasMaxLength(20);

                entity.Property(e => e.Length)
                    .HasColumnName("Length")
                    .HasMaxLength(20);

                entity.Property(e => e.Icon)
                    .HasColumnName("Icon")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<PrintPrice>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PrintId).HasColumnName("PrintId");

                entity.Property(e => e.Price)
                    .HasColumnName("Price")
                    .HasMaxLength(10);

                entity.Property(e => e.PhotographerId).HasColumnName("PhotographerId");

                //entity.HasOne(d => d.Photographer)
                //    .WithMany(p => p.PrintType)
                //    .HasForeignKey(d => d.PhotographerId)
                //    .HasConstraintName("FK_PrintPrice_User");

                //entity.HasOne(d => d.PrintType)
                //    .WithMany(p => p.PrintPrice)
                //    .HasForeignKey(d => d.PrintId)
                //    .HasConstraintName("FK_PrintPrice_PrintType");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<PhotoTag>(entity =>
            {
                entity.HasKey(x => new { x.PhotoId, x.TagId });
                entity.Property(e => e.RegisterDate).HasColumnType("datetime");

                entity.HasOne(x => x.Photo)
                    .WithMany(x => x.PhotoTag)
                    .HasForeignKey(x => x.PhotoId);

                entity.HasOne(x => x.Tag)
                    .WithMany(x => x.PhotoTag)
                    .HasForeignKey(x => x.TagId);
            });

            modelBuilder.Ignore<RootObject>();
        }
    }
}
