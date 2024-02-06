// Los atributos con /**/ pueden ser removidos si fuere necesario
using System;
using System.ComponentModel.DataAnnotations;

namespace BANCO.Models
{
    public class Movimiento:BancoBase
    {
        public string IdCuenta { get; set; }
        /**/public Cuenta Cuenta { get; set; }

        [Required(ErrorMessage ="Seleccione el Tipo de movimiento *")]
        public Tipo_Movimiento Tipo { get; set; }
        [Required(ErrorMessage = "Seleccione un tipo de servicio *")]
        public Tipo_PagoServicio TipoServicio { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Campo requerido *")]
        public double Monto { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:dd-MM-yyyy}")]
        public DateTime Fecha { get; set; }

        [StringLength(80)]
        public string Concepto { get; set; }

        // Propiedades adicionales para la transferencia
        public string IdCuentaOrigen { get; set; }

        [Display(Prompt = "Ingrese la cuenta destino")]
        [StringLength(12)]
        public string IdCuentaDestino { get; set; }

        public string IdUsuario { get; set; }
        //public string IdUsuarioDestino { get; set; }
    }
}
