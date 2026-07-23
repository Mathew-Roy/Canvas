using Library.Canvas.Models;

namespace Library.Canvas.Services
{
    public class CourseServiceProxy
    {
        private static CourseServiceProxy? _instance;
        private static object _instanceLock = new object();

        private CourseServiceProxy()
        {
            Courses = new List<Course>
            {
                new Course { Id = 1, Code = "COP4870", Name = "Full Stack Development", Description = "Building full stack applications", Term = Term.Fall, Year = 2026, Section = "001" },
                new Course { Id = 2, Code = "COP3330", Name = "Object Oriented Programming", Description = "OOP concepts in C++", Term = Term.Spring, Year = 2026, Section = "001" }
            };

            var demoStudent = StudentServiceProxy.Current.Students.FirstOrDefault();
            if (demoStudent != null)
            {
                Courses[0].Roster.Add(demoStudent);
                Courses[0].Assignments.Add(new Assignment
                {
                    Id = 1,
                    Name = "Intro Essay",
                    Description = "Write a short introduction.",
                    AvailablePoints = 100,
                    DueDate = new DateTime(2026, 9, 1)
                });
                var demoModule = new Module { Id = 1 };
                demoModule.Content.Add(new PageItem { Content = "Welcome to Full Stack Development!" });
                Courses[0].Modules.Add(demoModule);
            }
        }

        public static CourseServiceProxy Current
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new CourseServiceProxy();
                    return _instance;
                }
            }
        }

        private int _nextId = 3;
        public List<Course> Courses { get; set; }

        public void Add(Course course)
        {
            if (course.Id == 0)
                course.Id = _nextId++;
            Courses.Add(course);
        }

        public void Delete(int id)
        {
            var course = Courses.FirstOrDefault(c => c.Id == id);
            if (course != null)
                Courses.Remove(course);
        }
    }
}