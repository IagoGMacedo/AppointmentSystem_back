using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Utils.Extensions
{
    public static class ObjectExtensions
    {
        public static string? String(this object value)
        {
            return value == null ? string.Empty : value.ToString();
        }
    }
}
