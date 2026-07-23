using System.Collections.Generic;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace Maui.Canvas.ViewModels
{
    public class StudentCoursesViewModel
    {
        public Student? Student { get; private set; }
        public string StudentName => Student?.Name ?? "Student";
        public List<Course> Courses { get; private set; } = new();
        public bool HasNoCourses => Courses.Count == 0;

        public void Load(int studentId)
        {
            Student = StudentServiceProxy.Current.Students
                .FirstOrDefault(s => s.Id == studentId);

            if (Student != null)
            {
                Courses = CourseServiceProxy.Current.Courses
                    .Where(c => c.Roster.Any(s => s.Id == studentId))
                    .ToList();
            }
        }
    }
}