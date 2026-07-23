using System.Collections.Generic;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace Maui.Canvas.ViewModels
{
    public class TeacherCourseDetailViewModel
    {
        public Course? Course { get; private set; }
        public string CourseTitle => Course != null ? $"{Course.Name} ({Course.Code})" : "Course";
        public List<Student> Roster { get; private set; } = new();
        public List<Student> AvailableToAdd { get; private set; } = new();

        public void Load(int courseId)
        {
            Course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
            if (Course == null) return;

            Roster = Course.Roster.ToList();
            AvailableToAdd = StudentServiceProxy.Current.Students
                .Where(s => Course.Roster.All(r => r.Id != s.Id))
                .ToList();
        }
    }
}