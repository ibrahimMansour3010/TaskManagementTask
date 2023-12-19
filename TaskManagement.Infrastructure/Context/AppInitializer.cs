using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskManagement.Domain.Identity;
using TaskManagement.Domain.Shared.Constants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagement.Infrastructure.Context
{
    public class AppInitializer
    {
        private readonly AppDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AppInitializer> _logger;
        public AppInitializer(AppDBContext context, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, ILogger<AppInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }


        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
        public async Task TrySeedAsync()
        {
            if (!_roleManager.Roles.Any())
            {
                // roles
                await _roleManager.CreateAsync(new ApplicationRole() { Name = Roles.SuperAdmin.ToString() });
                await _roleManager.CreateAsync(new ApplicationRole() { Name = Roles.Admin.ToString() });
                await _roleManager.CreateAsync(new ApplicationRole() { Name = Roles.Basic.ToString() });
            }

            if (!_userManager.Users.Any())
            {
                // basic user
                var defaultUser = new ApplicationUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com",
                    EmailConfirmed = true,
                    Name = "Basic Admin"
                };
                if (_userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    var user = await _userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        await _userManager.CreateAsync(defaultUser, "P@SSW0RD");
                        await _userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
                    }
                }
                // super admin
                defaultUser = new ApplicationUser
                {
                    UserName = "superadmin@test.com",
                    Email = "superadmin@test.com",
                    EmailConfirmed = true,
                    Name = "Super Admin"
                };
                if (_userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    var user = await _userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        await _userManager.CreateAsync(defaultUser, "P@SSW0RD");
                        await _userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
                        await _userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                        await _userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                    }
                    await SeedClaimsForSuperAdmin();
                }
            }
            

        }
        private async Task SeedClaimsForSuperAdmin()
        {
            var adminRole = await _roleManager.FindByNameAsync("SuperAdmin");
            await AddPermissionClaim(adminRole, "Task");
        }

        public  async Task AddPermissionClaim(ApplicationRole role, string module)
        {
            var allClaims = await _roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
            }
        }
    }
}
