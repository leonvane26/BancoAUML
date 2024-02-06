using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace BANCO.Models
{
    public class BancoContext: DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
       
        public BancoContext(DbContextOptions options) : base(options)
        {

        }
    }
}
