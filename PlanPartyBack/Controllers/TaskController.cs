using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PlanPartyBack.Models;
using PlanPartyBack.Models.Request;
using PlanPartyBack.Services;
using System.Security.Claims;

namespace PlanPartyBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newTask = new TaskItem
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = userId,
                TaskName = request.TaskName,
                DurationInMinutes = request.DurationInMinutes,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddMinutes(request.DurationInMinutes),
                Status = "Em andamento",
                ElapsedMinutes = 0
            };

            await _taskService.CreateTaskAsync(newTask);

            return Ok(newTask);
        }

        [HttpPut("interrupt/{taskId}")]
        public async Task<IActionResult> InterruptTask(string taskId)
        {
            await _taskService.InterruptTaskAsync(taskId);
            return Ok(new { message = "Task interrupted" });
        }

        [HttpPut("complete/{taskId}")]
        public async Task<IActionResult> CompleteTask(string taskId)
        {
            await _taskService.CompleteTaskAsync(taskId);
            return Ok(new { message = "Task completed" });
        }

        [HttpGet("check-status/{taskId}")]
        public async Task<IActionResult> CheckTaskStatus(string taskId)
        {
            var task = await _taskService.GetTaskByIdAsync(taskId);
            if (task == null) return NotFound();

            // Verifica se o tempo já foi alcançado
            if (task.ElapsedMinutes >= task.DurationInMinutes)
            {
                task.Status = "Concluído";
            }
            else if (task.Status == "Em andamento")
            {
                task.Status = "Em andamento";
            }

            await _taskService.UpdateTaskAsync(taskId, task.TaskName, task.DurationInMinutes);

            return Ok(task);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetTaskHistory()
        {
            var userId = User.Identity.Name;
            var tasks = await _taskService.GetTaskHistoryAsync(userId);
            return Ok(tasks);
        }
    }
}
