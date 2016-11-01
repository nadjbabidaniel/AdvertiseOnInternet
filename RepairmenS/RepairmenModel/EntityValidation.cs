using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairmenModel
{
    public class EntityValidation<T> where T : Entity
    {
        public IEnumerable<ValidationResult> Validate(T entity)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(entity, null, null);
            Validator.TryValidateObject(entity, validationContext, validationResults, true);
            return validationResults;
        }
    }
}
