using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.ApplicationContracts.Dto.Role;
using TaskManagement.ApplicationContracts.Dto.Task;
using TaskManagement.ApplicationContracts.Dto.User;
using TaskManagement.Domain.Identity;
using TaskManagement.Domain.Models;

namespace TaskManagement.Application
{
    public class AutoMapperProfiler : Profile
    {
        public AutoMapperProfiler()
        {
            CreateMap<ApplicationUser, RegisterUserDto>().ReverseMap();
            CreateMap<ApplicationUser, LoginDto>().ReverseMap();
            CreateMap<ApplicationUser, UserLoggedInDto>().ReverseMap();
            CreateMap<TaskManagement.Domain.Models.Task, TaskDto>().ReverseMap();
            CreateMap<ApplicationRole, RoleDto>().ReverseMap();
        }
    }
}
