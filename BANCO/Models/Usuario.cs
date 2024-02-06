using BANCO.Models.Validar_FechaNac;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BANCO.Models
{
    public class Usuario:BancoBase
    {
        [Required(ErrorMessage = "Campo requerido *")]
        public override string Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido *")]
        public string Apellido { get; set; }

        //[DataType(DataType.Date)]
        [Required(ErrorMessage = "Campo requerido *")]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])-(0[1-9]|1[0-2])-([12]\d{3})$", ErrorMessage = "El formato de fecha debe ser dd-mm-yyyy")]
        [CustomValidation(typeof(FechaNacValidacion), "ValidarFechaNac")]
        [Display(Prompt ="15-06-2001")]
        [StringLength(10)]

        public string FechaNacimiento { get; set; }


        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage ="Campo requerido *")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "Campo requerido *")]
        [RegularExpression(@"^\d{8}-?\d$", ErrorMessage = "Formato inválido. El formato correcto es: 00000000-0")]
        [Display(Prompt = "01234567-1")]
        [StringLength(10)]
        public string DUI { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Campo requerido *")]
        [Display(Prompt = "2294-9635")]
        [RegularExpression(@"^\d{4}-?\d{4}$", ErrorMessage = "Formato inválido. El formato correcto es: 0000-0000")]
        [MaxLength(9)]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Campo requerido *")]
        [StringLength(60)]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Campo requerido *")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Campo requerido *")]
        public string Contraseña { get; set; }

        

    }

}
