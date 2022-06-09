namespace DbEHealthcare
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Entities;

    public partial class DbEHealthCare : DbContext
    {
        public DbEHealthCare()
            : base("name=DbEHealthCare")
        {
        }

        public virtual DbSet<BacSi> BacSis { get; set; }
        public virtual DbSet<BenhNhan> BenhNhans { get; set; }
        public virtual DbSet<ChiTietTuVan> ChiTietTuVans { get; set; }
        public virtual DbSet<ChuyenKhoa> ChuyenKhoas { get; set; }
        public virtual DbSet<LichTuVan> LichTuVans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BacSi>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<BacSi>()
                .Property(e => e.matkhau)
                .IsUnicode(false);

            modelBuilder.Entity<BacSi>()
                .Property(e => e.hinhanh)
                .IsUnicode(false);

            modelBuilder.Entity<BacSi>()
                .HasMany(e => e.LichTuVans)
                .WithRequired(e => e.BacSi)
                .HasForeignKey(e => e.email_BS)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BenhNhan>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<BenhNhan>()
                .Property(e => e.matkhau)
                .IsUnicode(false);

            modelBuilder.Entity<BenhNhan>()
                .Property(e => e.sdt)
                .IsUnicode(false);

            modelBuilder.Entity<BenhNhan>()
                .Property(e => e.hinhanh)
                .IsUnicode(false);

            modelBuilder.Entity<BenhNhan>()
                .HasMany(e => e.LichTuVans)
                .WithOptional(e => e.BenhNhan)
                .HasForeignKey(e => e.email_BN);

            modelBuilder.Entity<LichTuVan>()
                .Property(e => e.email_BS)
                .IsUnicode(false);

            modelBuilder.Entity<LichTuVan>()
                .Property(e => e.email_BN)
                .IsUnicode(false);
        }
    }
}
