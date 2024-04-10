using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Authentication.Basic.Attributes
{
    public class BasicAuthorizationAttributes : AuthorizeAttribute
    {
        public BasicAuthorizationAttributes()
        {
            AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme;
        }
    }
}
