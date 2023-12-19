using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.ApplicationContracts.Dto.Task;
using TaskManagement.ApplicationContracts.Tasks;
using TaskManagement.Domain.Models;
using TaskManagement.Domain.Respiratory;
using TaskManagement.Domain.Shared.Helpers;
using TaskManagement.Domain.UnitOfWork;
using static TaskManagement.Domain.Shared.Constants.Permissions;

namespace TaskManagement.Application.Task
{
    public class TaskServices: ITaskServices
    {
        private readonly IGenericRepo<Domain.Models.Task> _taskRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TaskServices(IGenericRepo<TaskManagement.Domain.Models.Task> taskRepo,
            IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _taskRepo = _unitOfWork.Repository<Domain.Models.Task>();
            _mapper = mapper;
        }

        public async Task<ResponseDto<List<TaskDto>>> GetAll()
        {
            var response = new ResponseDto<List<TaskDto>>();
            var data =  _taskRepo.Get(filter: x => !x.IsDeleted,orderBy:x=>x.OrderByDescending(c=>c.CreationDate));
            var result = data.ProjectTo<TaskDto>(_mapper.ConfigurationProvider);
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Message = "success";
            response.Data = result.ToList();
            return response;
        }
        public async Task<ResponseDto<PaginatedList<TaskDto>>> GetAllPaged(int pageNumber, int pageSize)
        {
            var response = new ResponseDto<PaginatedList<TaskDto>>();
            var data = await _taskRepo.GetPaginatedAsync(filter: x => !x.IsDeleted,
                orderBy:x=>x.OrderByDescending(z=>z.CreationDate),
                page: pageNumber,
                pageSize: pageSize);
            var result = data.ProjectTo<TaskDto>(_mapper.ConfigurationProvider);
            var count = await _taskRepo.CountAsync(filter: x => !x.IsDeleted);
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Message = "success";
            response.Data = new PaginatedList<TaskDto>(result.ToList(), count, pageNumber, pageSize);

            return response;
        }
        public async Task<ResponseDto<TaskDto>> GetById(Guid id)
        {
            var response = new ResponseDto<TaskDto>();
            var task = await _taskRepo.GetOneAsyncNoTrack(x => x.Id == id);

            if (task is null)
            {
                response.IsSuccess = false;
                response.StatusCode = 404;
                response.Message = "not found";
                return response;
            }

            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Message = "success";
            response.Data = _mapper.Map<TaskDto>(task);
            return response;
        }
        public async Task<ResponseDto<TaskDto>> Add(TaskDto dto)
        {
            var response = new ResponseDto<TaskDto>();
            if(dto is null)
            {
                response.IsSuccess = false;
                response.StatusCode = 404;
                response.Message = "not found";
                return response;
            }
            var task = _mapper.Map<TaskManagement.Domain.Models.Task>(dto);
            await _taskRepo.InsertAsync(task);
            await _unitOfWork.CompleteAsync();

            response = await GetById(task.Id);

            return response;
        }
        public async Task<ResponseDto<TaskDto>> Update(TaskDto dto)
        {
            var response = new ResponseDto<TaskDto>();
            if(dto is null)
            {
                response.IsSuccess = false;
                response.StatusCode = 404;
                response.Message = "not found";
                return response;
            }
            var task = await _taskRepo.GetOneAsyncNoTrack(x => x.Id == dto.Id);
            if (task is null)
            {
                response.IsSuccess = false;
                response.StatusCode = 404;
                response.Message = "not found";
                return response;
            }
            var updateTask = _mapper.Map<TaskManagement.Domain.Models.Task>(dto);
            _taskRepo.Update(updateTask);
            await _unitOfWork.CompleteAsync();

            response = await GetById(task.Id);

            return response;
        }
        public async Task<ResponseDto<TaskDto>> Delete(Guid id)
        {
            var response = new ResponseDto<TaskDto>();
            var task = await _taskRepo.GetOneAsyncNoTrack(x => x.Id == id);
            if (task is null)
            {
                response.IsSuccess = false;
                response.StatusCode = 404;
                response.Message = "not found";
                return response;
            }
            task.IsDeleted = true;
            _taskRepo.Update(task);
            await _unitOfWork.CompleteAsync();

            response = await GetById(task.Id);

            return response;
        }
    }
}
