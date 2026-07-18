namespace Library.Canvas.Models
{
    public enum Term
    {
        Spring,
        Summer,
        Fall,
        Winter
    }

    public class Course
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        // #25 - Semester
        public Term Term { get; set; }
        public int Year { get; set; }
        public string Semester => $"{Term} {Year}";   // computed display, e.g. "Fall 2026"

        // #27 - Section
        public string? Section { get; set; }

        public List<Student> Roster { get; set; } = new List<Student>();
        public List<Module> Modules { get; set; } = new List<Module>();
        public List<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}