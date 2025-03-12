namespace Price_Comparison_Website.Models
{
    public class Notification
    {
        public int Id { get; set; } // PK
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsGlobal {get; set;} 
        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    }
}