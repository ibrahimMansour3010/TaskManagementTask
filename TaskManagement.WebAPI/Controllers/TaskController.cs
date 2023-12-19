using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.ApplicationContracts.Dto.Task;
using TaskManagement.ApplicationContracts.Dto.User;
using TaskManagement.ApplicationContracts.Tasks;
using TaskManagement.Domain.Shared.Constants;

namespace TaskManagement.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize()]

    public class TaskController : ControllerBase
    {
        private readonly ITaskServices _taskServices;

        public TaskController(ITaskServices taskServices)
        {
            this._taskServices = taskServices;
        }
        [HttpGet]
        [Route("GetAll")]
        [Authorize(Policy = Permissions.Task.View)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _taskServices.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet]
        [Route("GetAllPaged")]
        public async Task<IActionResult> GetAllPaged([FromQuery]int page=1,int pageSize =3)
        {
            try
            {
                var result = await _taskServices.GetAllPaged(page,pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            try
            {
                var result = await _taskServices.GetById(Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add(TaskDto taskDto)
        {
            try
            {
                var result = await _taskServices.Add(taskDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(TaskDto taskDto)
        {
            try
            {
                var result = await _taskServices.Update(taskDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            try
            {
                var result = await _taskServices.Delete(Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
