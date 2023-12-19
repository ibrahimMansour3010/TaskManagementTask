using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.ApplicationContracts.Dto.Task;
using TaskManagement.Domain.Shared.Helpers;

namespace TaskManagement.ApplicationContracts.Tasks
{
    public interface ITaskServices
    {
        Task<ResponseDto<List<TaskDto>>> GetAll();
        Task<ResponseDto<PaginatedList<TaskDto>>> GetAllPaged(int pageNumber, int pageSize);
        Task<ResponseDto<TaskDto>> GetById(Guid id);
        Task<ResponseDto<TaskDto>> Add(TaskDto dto);
        Task<ResponseDto<TaskDto>> Update(TaskDto dto);
        Task<ResponseDto<TaskDto>> Delete(Guid id);
    }
}
