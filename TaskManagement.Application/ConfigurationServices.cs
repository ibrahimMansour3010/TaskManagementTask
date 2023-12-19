using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Account;
using TaskManagement.Application.Task;
using TaskManagement.ApplicationContracts.Account;
using TaskManagement.ApplicationContracts.Permissions;
using TaskManagement.ApplicationContracts.Tasks;

namespace TaskManagement.Application
{
    public static class ConfigurationServices
    {
        public static void ApplicationService(this IServiceCollection services)
        {
            services.AddScoped<IAccountService,AccountService>();
            services.AddScoped<ITaskServices, TaskServices>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }
    }
}
