using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.ApplicationContracts.Dto.User
{
    public class LoginDto
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
