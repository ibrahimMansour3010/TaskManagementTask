using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.ApplicationContracts.Account;
using TaskManagement.ApplicationContracts.Dto.User;
using TaskManagement.Domain.Identity;
using TaskManagement.Domain.Shared.Constants;
using TaskManagement.Domain.Shared.Helpers;

namespace TaskManagement.Application.Account
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;

        public AccountService(UserManager<ApplicationUser> userManager, IMapper mapper,
            IOptions<JWT> jwt, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwt = jwt.Value;
            _roleManager = roleManager;
        }

        public async Task<ResponseDto<RegisterUserDto>> Register(RegisterUserDto userDto)
        {
            var response = new ResponseDto<RegisterUserDto>();
            var user = await _userManager.FindByEmailAsync(userDto.Email);
            if (user != null)
            {
                response.IsSuccess = false;
                response.Message = "this email already exists";
                response.StatusCode = 0;
                response.Data = null;
                return response;
            }
            user = await _userManager.FindByNameAsync(userDto.UserName);
            if (user != null)
            {
                response.IsSuccess = false;
                response.Message = "this username already exists";
                response.StatusCode = 1;
                response.Data = null;
                return response;
            }
            user = _mapper.Map<ApplicationUser>(userDto);
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                response.IsSuccess = false;
                foreach (var error in result.Errors)
                {
                    response.Message = error.Description;
                }
                response.StatusCode = 2;
                response.Data = null;
                return response;
            }

            response.IsSuccess = true;

            response.Message = "success";
            response.StatusCode = 200;
            response.Data = userDto;
            return response;
        }
        public async Task<ResponseDto<UserLoggedInDto>> Login(LoginDto userDto)
        {
            var response = new ResponseDto<UserLoggedInDto>();
            var user = await _userManager.FindByEmailAsync(userDto.UsernameOrEmail);
            if (user is null) 
                user = await _userManager.FindByNameAsync(userDto.UsernameOrEmail);
            if(user is null)
            {
                response.IsSuccess = false;
                response.Message = "this email or username is not found";
                response.StatusCode = 3;
                response.Data = null;
                return response;
            }
            var result = await _userManager.CheckPasswordAsync(user, userDto.Password);
            if (!result)
            {
                response.IsSuccess = false;
                response.Message = "incorrect password";
                response.StatusCode = 4;
                response.Data = null;
                return response;
            }
            var userLoggedin = _mapper.Map<UserLoggedInDto>(user);
            var jwtSecurityToken = await CreateJwtToken(user);
            userLoggedin.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            userLoggedin.Roles = await _userManager.GetRolesAsync(user);
            foreach (var role in userLoggedin.Roles)
            {
                var AppRole = await _roleManager.FindByNameAsync(role);
                var roleClaim = await _roleManager.GetClaimsAsync(AppRole);
                userLoggedin.Permissions.AddRange(roleClaim.Select(x=>x.Value).ToList());
            }
            response.IsSuccess = true;
            response.Message = "success";
            response.StatusCode = 200;
            response.Data = userLoggedin;
            return response;
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
                var AppRole = await _roleManager.FindByNameAsync(role);
                var roleClaim = await _roleManager.GetClaimsAsync(AppRole);
                permissionClaims.AddRange(roleClaim);
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("userRoles", string.Join(',',roles)),
                new Claim("Permission", string.Join(',',permissionClaims)),
            }
            .Union(userClaims)
            .Union(permissionClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
