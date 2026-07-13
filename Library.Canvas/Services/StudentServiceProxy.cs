using Library.Canvas.Models;

namespace Library.Canvas.Services
{
    public class StudentServiceProxy
    {
        private static StudentServiceProxy? _instance;
        private static object _instanceLock = new object();

        private StudentServiceProxy()
        {
            Students = new List<Student>
            {
                new Student { Id = 1, Name = "Alice Smith", Code = "as21a", Classification = "Junior" },
                new Student { Id = 2, Name = "Bob Jones", Code = "bj22b", Classification = "Senior" },
                new Student { Id = 3, Name = "Carol White", Code = "cw23c", Classification = "Sophomore" }
            };
        }

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