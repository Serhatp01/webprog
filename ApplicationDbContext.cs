using Microsoft.EntityFrameworkCore;
using OdevDeneme1.Models;

namespace OdevDeneme1.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{ }

		public DbSet<Salon> Salonlar { get; set; }
		public DbSet<Hizmet> Hizmetler { get; set; }
		public DbSet<Antrenor> Antrenorler { get; set; }
        public DbSet<Admin> Adminler { get; set; }
        public DbSet<Uye> Uyeler { get; set; }
		public DbSet<Randevu> Randevular { get; set; }
		public DbSet<AntrenorHizmet> AntrenorHizmetler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AntrenorHizmet>()
                .HasKey(x => new { x.AntrenorId, x.HizmetId });

            modelBuilder.Entity<AntrenorHizmet>()
                .HasOne(ah => ah.Antrenor)
                .WithMany(a => a.AntrenorHizmetler)
                .HasForeignKey(ah => ah.AntrenorId);

            modelBuilder.Entity<AntrenorHizmet>()
                .HasOne(ah => ah.Hizmet)
                .WithMany(h => h.AntrenorHizmetler)
                .HasForeignKey(ah => ah.HizmetId);

            base.OnModelCreating(modelBuilder);
        }

    }

}
