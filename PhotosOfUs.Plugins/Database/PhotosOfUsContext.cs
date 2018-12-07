using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace PhotosOfUs.Model.Models
{
    public partial class PhotosOfUsContext : DbContext
    {
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<BrandAccount> BrandAccount { get; set; }
        public virtual DbSet<Card> Card { get; set; }
        public virtual DbSet<Folder> Folder { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<Photo> Photo { get; set; }
        public virtual DbSet<PhotoTag> PhotoTag { get; set; }
        public virtual DbSet<PrintPrice> PrintPrice { get; set; }
        public virtual DbSet<PrintType> PrintType { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserIdentity> UserIdentity { get; set; }

        public PhotosOfUsContext(DbContextOptions<PhotosOfUsContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.Address1)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Address2)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(125)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
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
            });

            modelBuilder.Entity<BrandAccount>(entity =>
            {
                entity.Property(e => e.BrandAccountId)
                    .HasColumnName("BrandAccountID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AccentColorCode)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.Property(e => e.BackgroundColorCode)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.Property(e => e.BodyColorCode)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.FeaturedImageUrl)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.HeaderColorCode)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.Property(e => e.HomepageTemplate)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.OverlayColorCode)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.Property(e => e.OverlayOpacity)
                    .IsRequired()
                    .HasMaxLength(3);

                entity.Property(e => e.PageTitle)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PersonalLogoUrl)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.SeparatorStyle)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                //entity.HasOne(d => d.User)
                //    .WithMany(p => p.BrandAccount)
                //    .HasForeignKey(d => d.UserId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_BrandAccount_User");
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
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

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
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(128)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Total).HasColumnType("decimal(19, 4)");

                //entity.HasOne(d => d.BillingAddress)
                //    .WithMany(p => p.OrderBillingAddress)
                //    .HasForeignKey(d => d.BillingAddressId)
                //    .HasConstraintName("FK_Order_BillingAddressId");

                //entity.HasOne(d => d.ShippingAddress)
                //    .WithMany(p => p.OrderShippingAddress)
                //    .HasForeignKey(d => d.ShippingAddressId)
                //    .HasConstraintName("FK_Order_ShippingAddressId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(19, 4)");

                //entity.HasOne(d => d.Order)
                //    .WithMany(p => p.OrderDetail)
                //    .HasForeignKey(d => d.OrderId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_OrderDetail_Order");

                //entity.HasOne(d => d.Photo)
                //    .WithMany(p => p.OrderDetail)
                //    .HasForeignKey(d => d.PhotoId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_OrderDetail_Photo");

                //entity.HasOne(d => d.PrintType)
                //    .WithMany(p => p.OrderDetail)
                //    .HasForeignKey(d => d.PrintTypeId)
                //    .HasConstraintName("FK_OrderDetail_PrintType");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.SuggestedTags)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

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

                entity.Ignore(x => x.FolderName);
                entity.Ignore(x => x.Stream);
                entity.Ignore(x => x.FileSize);
                entity.Ignore(x => x.Resolution);
                entity.Ignore(x => x.ThumbnailUrl);
                entity.Ignore(x => x.WaterMarkUrl);
                entity.Property(x => x.SuggestedTags)
                    .HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<RootObject>(x));
            });

            modelBuilder.Entity<PhotoTag>(entity =>
            {
                entity.HasKey(e => new { e.PhotoId, e.TagId });

                entity.Property(e => e.RegisterDate).HasColumnType("datetime");

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.PhotoTag)
                    .HasForeignKey(d => d.PhotoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhotoTag_PhotoId");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.PhotoTag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhotoTag_TagId");
            });

            modelBuilder.Entity<PrintPrice>(entity =>
            {
                //entity.HasOne(d => d.Photo)
                //    .WithMany(p => p.PrintPrice)
                //    .HasForeignKey(d => d.PhotoId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_PrintPrice_PrintType");

                entity.HasOne(d => d.Photographer)
                    .WithMany(p => p.PrintPrice)
                    .HasForeignKey(d => d.PhotographerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrintPrice_User");
            });

            modelBuilder.Entity<PrintType>(entity =>
            {
                entity.Property(e => e.Height).HasMaxLength(20);

                entity.Property(e => e.Icon).HasMaxLength(20);

                entity.Property(e => e.Length).HasMaxLength(20);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AzureId)
                    .HasColumnName("AzureID")
                    .HasMaxLength(128);

                entity.Property(e => e.Bio)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DisplayName).HasMaxLength(128);

                entity.Property(e => e.Dribbble).HasMaxLength(128);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Facebook).HasMaxLength(128);

                entity.Property(e => e.FirstName).HasMaxLength(128);

                entity.Property(e => e.Instagram).HasMaxLength(128);

                entity.Property(e => e.JobPosition).HasMaxLength(128);

                //entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.LastName).HasMaxLength(128);

                entity.Property(e => e.ProfilePhotoUrl)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.TemplateSelected).HasDefaultValueSql("((1))");

                entity.Property(e => e.Twitter).HasMaxLength(128);
            });

            modelBuilder.Entity<UserIdentity>(entity =>
            {
                entity.HasKey(e => e.AzureID);
                entity.Property(e => e.AzureID)
                    .ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                //entity.Property(e => e.UserId).HasColumnName("UserID");
            });
        }
    }
}
