using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Domain.Shared.Constants
{
    public class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
        {
            $"Permissions.{module}.Create",
            $"Permissions.{module}.View",
            $"Permissions.{module}.Edit",
            $"Permissions.{module}.Delete",
        };
        }
        public static class Task
        {
            public const string View = "Permissions.Task.View";
            public const string Create = "Permissions.Task.Create";
            public const string Edit = "Permissions.Task.Edit";
            public const string Delete = "Permissions.Task.Delete";
        }
    }
}
