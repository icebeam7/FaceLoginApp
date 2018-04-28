using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;
using FaceLoginApp.Modelos;

namespace FaceLoginApp.Datos
{
    public class BaseDatos : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }

        private readonly string rutaBD;

        public BaseDatos(string rutaBD)
        {
            this.rutaBD = rutaBD;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = DependencyService.Get<IBaseDatos>().GetDatabasePath();
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
}