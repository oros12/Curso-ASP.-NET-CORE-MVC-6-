using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Validaciones
{
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            var PrimerLetra = value.ToString ()[0].ToString ();
            if(PrimerLetra!=PrimerLetra.ToUpper())
            {
                return new ValidationResult("La primer letra debe ser mayuscula");
            }
            return ValidationResult.Success;
        }
    }

}
