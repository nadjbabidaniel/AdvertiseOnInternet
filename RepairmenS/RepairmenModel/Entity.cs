using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairmenModel
{
    public abstract class Entity
    {
        public virtual IEnumerable<ValidationResult> Validate()
        {
            return EntityValidator.ValidateEntity(this);
        }
    }
}
