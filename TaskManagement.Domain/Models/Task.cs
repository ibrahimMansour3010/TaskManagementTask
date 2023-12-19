using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Shared.Enums;

namespace TaskManagement.Domain.Models
{
    public class Task:BaseEntityModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskType Type { get; set; }
    }
}
