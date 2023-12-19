using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Shared.Enums;

namespace TaskManagement.ApplicationContracts.Dto.User
{
    public class UserLoggedInDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public IList<string> Roles { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
        public Gender Gender { get; set; }
    }
}
