using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Utils.UserContext
{
    /// <summary>
    /// Informações da origem da requisição HTTP.
    /// </summary>
    public class SourceInfo : ISourceInfo
    {
        /// <summary>
        /// Contém HEADERS da requisição.
        /// </summary>
        public Hashtable Data { get; set; }

        /// <summary>
        /// Origem da requisição.
        /// </summary>
        public IPAddress IP { get; set; }
    }
}
