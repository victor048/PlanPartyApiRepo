using MongoDB.Driver;
using PlanPartyBack.Models;

namespace PlanPartyBack.Services
{
    public class TaskService
    {
        private readonly IMongoCollection<TaskItem> _taskCollection;

        public TaskService(IMongoDatabase database)
        {
            _taskCollection = database.GetCollection<TaskItem>("Tasks");
        }

        public async Task CreateTaskAsync(TaskItem task)
        {
            task.Status = "Em andamento";
            task.StartTime = DateTime.Now;
            task.ElapsedMinutes = 0;
            await _taskCollection.InsertOneAsync(task);
        }

        public async Task InterruptTaskAsync(string taskId)
        {
            var task = await _taskCollection.Find(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task != null && task.Status == "Em andamento")
            {
                var elapsedTime = (DateTime.Now - task.StartTime).TotalMinutes;
                var update = Builders<TaskItem>.Update
                    .Set(t => t.Status, "Interrompido")
                    .Set(t => t.EndTime, DateTime.Now)
                    .Set(t => t.ElapsedMinutes, (int)elapsedTime);

                await _taskCollection.UpdateOneAsync(t => t.Id == taskId, update);
            }
        }

        public async Task CompleteTaskAsync(string taskId)
        {
            var task = await _taskCollection.Find(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task != null && task.Status == "Em andamento")
            {
                var elapsedTime = (DateTime.Now - task.StartTime).TotalMinutes;
                var update = Builders<TaskItem>.Update
                    .Set(t => t.Status, "Concluído")
                    .Set(t => t.EndTime, DateTime.Now)
                    .Set(t => t.ElapsedMinutes, (int)elapsedTime);

                await _taskCollection.UpdateOneAsync(t => t.Id == taskId, update);
            }
        }

        public async Task UpdateTaskAsync(string taskId, string newTaskName, int newDuration)
        {
            var task = await _taskCollection.Find(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task != null && task.Status != "Concluído")
            {
                var update = Builders<TaskItem>.Update
                    .Set(t => t.TaskName, newTaskName)
                    .Set(t => t.DurationInMinutes, newDuration);

                await _taskCollection.UpdateOneAsync(t => t.Id == taskId, update);
            }
        }

        // Método para obter o histórico de tarefas de um usuário
        public async Task<List<TaskItem>> GetTaskHistoryAsync(string userId)
        {
            return await _taskCollection.Find(t => t.UserId == userId).ToListAsync();
        }

        public async Task<TaskItem> GetTaskByIdAsync(string taskId)
        {
            return await _taskCollection.Find(t => t.Id == taskId).FirstOrDefaultAsync();
        }

    }
}
