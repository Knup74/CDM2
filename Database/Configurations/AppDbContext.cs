using Microsoft.EntityFrameworkCore;
using CDM.Database.Models;


namespace CDM.Database;


public class AppDbContext : DbContext
{
public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}


public DbSet<Coproprietaire> Coproprietaires => Set<Coproprietaire>();
public DbSet<Lot> Lots => Set<Lot>();
public DbSet<SousCopropriete> SousCoproprietes => Set<SousCopropriete>();
public DbSet<Trimestre> Trimestres => Set<Trimestre>();
public DbSet<ChargeTrimestre> ChargeTrimestres => Set<ChargeTrimestre>();
public DbSet<ChargeTrimestreSousCopro> ChargeTrimestreSousCopros => Set<ChargeTrimestreSousCopro>();
public DbSet<AppelDeFond> AppelsDeFonds => Set<AppelDeFond>();


protected override void OnModelCreating(ModelBuilder modelBuilder)
{
base.OnModelCreating(modelBuilder);


// Clé composite pour la table pivot
modelBuilder.Entity<ChargeTrimestreSousCopro>()
.HasKey(cts => new { cts.ChargeTrimestreId, cts.SousCoproprieteId });


modelBuilder.Entity<ChargeTrimestreSousCopro>()
.HasOne(cts => cts.ChargeTrimestre)
.WithMany(c => c.SousCopros)
.HasForeignKey(cts => cts.ChargeTrimestreId);


modelBuilder.Entity<ChargeTrimestreSousCopro>()
.HasOne(cts => cts.SousCopropriete)
.WithMany(s => s.ChargeAssociations)
.HasForeignKey(cts => cts.SousCoproprieteId);


// Contrainte: Numero du trimestre entre 1 et 4
modelBuilder.Entity<Trimestre>()
.Property(t => t.Numero)
.HasConversion<int>()
.IsRequired();


// Index utiles
modelBuilder.Entity<Lot>()
.HasIndex(l => l.NumeroLot)
.IsUnique();


modelBuilder.Entity<Coproprietaire>()
.HasIndex(c => c.Nom);
}
}