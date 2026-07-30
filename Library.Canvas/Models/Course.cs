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

        public Term Term { get; set; }
        public int Year { get; set; }
        public string Semester => $"{Term} {Year}";   // computed display, e.g. "Fall 2026"

        public string? Section { get; set; }

        public List<Student> Roster { get; set; } = new List<Student>();
        public List<Module> Modules { get; set; } = new List<Module>();
        public List<Assignment> Assignments { get; set; } = new List<Assignment>();
        public List<AssignmentGroup> AssignmentGroups { get; set; } = new List<AssignmentGroup>();
    
        public List<string> Announcements { get; set; } = new List<string>();
    
        public int GradeA { get; set; } = 90;
        public int GradeB { get; set; } = 80;
        public int GradeC { get; set; } = 70;
        public int GradeD { get; set; } = 60;

        public string ColorA { get; set; } = "#2E7D32";
        public string ColorB { get; set; } = "#1565C0";
        public string ColorC { get; set; } = "#F9A825";
        public string ColorD { get; set; } = "#EF6C00";
        public string ColorF { get; set; } = "#C62828";
    }
}