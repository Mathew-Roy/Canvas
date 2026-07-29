using Library.Canvas.Models;
using Library.Canvas.Database;

namespace Library.Canvas.Services
{
    public class StudentServiceProxy
    {
        private static StudentServiceProxy? _instance;
        private static object _instanceLock = new object();

        private StudentServiceProxy()
        {
            Students = CanvasDbContext.Current.GetStudents();
            if (Students.Count == 0)
            {
                Students = Seed();
                CanvasDbContext.Current.SaveStudents(Students);
            }
        }

        private List<Student> Seed() => new List<Student>
        {
            // >>> MOVE your existing seeded students here (Alice, Bob, Carol) <
        };

        public void Save() => CanvasDbContext.Current.SaveStudents(Students);

        public static StudentServiceProxy Current
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new StudentServiceProxy();
                    return _instance;
                }
            }
        }

        public List<Student> Students { get; set; }

        public void Add(Student student)
        {
            if (student.Id == 0)
                student.Id = Students.Count + 1;
            Students.Add(student);
        }
    }
}