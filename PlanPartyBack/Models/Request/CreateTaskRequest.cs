namespace PlanPartyBack.Models.Request
{
    public class CreateTaskRequest
    {
        public string TaskName { get; set; }
        public int DurationInMinutes { get; set; }
        public string Status { get; set; }
        public int ElapsedMinutes { get; set; }
    }
}
