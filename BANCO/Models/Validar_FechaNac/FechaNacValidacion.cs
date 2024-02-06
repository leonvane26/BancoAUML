using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System;

namespace BANCO.Models.Validar_FechaNac
{
    public class FechaNacValidacion
    {
        public static ValidationResult ValidarFechaNac(string fechaNacimiento, ValidationContext validationContext)
        {
            if (!DateTime.TryParseExact(fechaNacimiento, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
            {
                //return new ValidationResult("El formato de fecha debe ser dd-mm-yyyy");
            }

            DateTime fechaMin = new DateTime(1930, 1, 1);
            DateTime fechaMax = new DateTime(2005, 6, 1);

            if (fecha < fechaMin || fecha > fechaMax)
            {
                return new ValidationResult("La fecha de nacimiento debe estar entre 01-01-1930 y 01-06-2005");
            }

            return ValidationResult.Success;
        }
    }
}
