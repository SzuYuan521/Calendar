namespace Calendar.Models
{
    public class Calendars
    {
        public int CalendarId { get; set; }

        public string Title { get; set; }

        public DateTime DateTime { get; set; }

        public bool HasReminder { get; set; }

        public int ReminderMinutes { get; set;}
    }
}