using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairmenModel
{
    public class EntityValidator
    {
        public static IEnumerable<ValidationResult> ValidateEntity<T>(T entity) where T : Entity
        {
            return new EntityValidation<T>().Validate(entity);
        }
    }
}
