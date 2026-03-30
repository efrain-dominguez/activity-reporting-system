using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS.Domain.Exceptions
{
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message)
        : base(message)
        {
        }

        public UnauthorizedException(string action, string role)
            : base($"User with role '{role}' is not authorized to perform action '{action}'.")
        {
        }

        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
