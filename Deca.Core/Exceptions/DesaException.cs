using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Exceptions
{
    public class DesaException(int statusCode, string message) : Exception(message)
    {
        public int StatusCode { get; private set; } = statusCode;
    }
}
