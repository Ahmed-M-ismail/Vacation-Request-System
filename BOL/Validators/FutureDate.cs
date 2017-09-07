using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BLL
{
   
        public class FutureDate : ValidationAttribute
        {

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null)
                {

                    return ValidationResult.Success;

                }

                else
                {
                    DateTime dt = (DateTime) value;
                    if (dt >= DateTime.UtcNow)
                    {
                        return ValidationResult.Success;
                    }

                    return new ValidationResult(ErrorMessage ?? "Make sure your date is >= than today");
                }
            }


        }
    }

