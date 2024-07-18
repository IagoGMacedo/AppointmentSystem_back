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
    /// Interface com informações da origem da requisição HTTP.
    /// </summary>
    public interface ISourceInfo
    {
        /// <summary>
        /// Contém HEADERS da requisição.
        /// </summary>
        Hashtable Data { get; set; }

        /// <summary>
        /// Origem da requisição.
        /// </summary>
        IPAddress IP { get; set; }
    }
}
