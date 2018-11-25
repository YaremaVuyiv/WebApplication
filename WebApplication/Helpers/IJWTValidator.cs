using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Helpers
{
    public interface IJWTValidator
    {
        Task<bool> IsTokenValid(IHeaderDictionary headerDictionary, HttpContext httpContext);
    }
}
