using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.ApplicationContracts.Dto.Role
{
    public class RoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Permissions { get; set; }
    }
}
