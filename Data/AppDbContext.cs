using Microsoft.EntityFrameworkCore;
using VotaYa.Models;

namespace VotaYa.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<Opcion> Opciones { get; set; }
    }
}
