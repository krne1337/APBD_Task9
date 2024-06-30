using Task9.Models;

namespace Task9.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Prescription_Medicament> Prescription_Medicaments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Prescription_Medicament>()
                .HasKey(pm => new { pm.IdPrescription, pm.IdMedicament });

            modelBuilder.Entity<Prescription_Medicament>()
                .HasOne(pm => pm.Prescription)
                .WithMany(p => p.Prescription_Medicaments)
                .HasForeignKey(pm => pm.IdPrescription);

            modelBuilder.Entity<Prescription_Medicament>()
                .HasOne(pm => pm.Medicament)
                .WithMany(m => m.Prescription_Medicaments)
                .HasForeignKey(pm => pm.IdMedicament);

            modelBuilder.Entity<Prescription>()
                .Property(p => p.RowVersion)
                .IsRowVersion();
        }

    }

}
