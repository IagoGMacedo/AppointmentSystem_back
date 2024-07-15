using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Utils.Exceptions
{
    [Serializable]
    public class BusinessListException : Exception
    {
        public List<string> Messages { get; set; }

        public BusinessListException() { }

        public BusinessListException(IEnumerable<string> messages)
        {
            Messages = messages.ToList();
        }

        protected BusinessListException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
