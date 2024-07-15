using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Entity.Entity
{
    public abstract class IdEntity<T> : IEntity
    {
        public T Id { get; set; }

    }
}
