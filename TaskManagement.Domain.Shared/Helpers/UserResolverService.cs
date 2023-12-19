﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Domain.Shared.Helpers
{
    public class UserResolverService
    {
        public readonly IHttpContextAccessor _context;

        public UserResolverService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetGivenName()
        {
            return _context.HttpContext.User.FindFirst(ClaimTypes.GivenName).Value;
        }

        public string GetSurname()
        {
            return _context.HttpContext.User.FindFirst(ClaimTypes.Surname).Value;
        }

        public string GetNameIdentifier()
        {
            if (_context.HttpContext?.User is null)
                return string.Empty;
            return _context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public string GetEmails()
        {
            return _context.HttpContext.User.FindFirst("emails").Value;
        }
    }
}
