using System.Collections.Generic;

namespace BANCO.Models
{
    public class PrintPDFModel
    {
        public Cuenta Cuenta { get; set; }
        public List<Movimiento> Movimientos { get; set; }
    }
}