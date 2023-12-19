using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.ApplicationContracts.Dto.User;
using TaskManagement.Domain.Shared.Helpers;

namespace TaskManagement.ApplicationContracts.Account
{
    public interface IAccountService
    {
        Task<ResponseDto<RegisterUserDto>> Register(RegisterUserDto userDto);
        Task<ResponseDto<UserLoggedInDto>> Login(LoginDto userDto);
    }
}
