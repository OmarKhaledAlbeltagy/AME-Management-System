namespace AMEKSA.Models
{
    public class TedReport
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string RegistrationDate { get; set; }

        public string AttendanceTime { get; set; }

        public bool Registered { get; set; } = false;

        public bool Attended { get; set; } = false;
    }
}
