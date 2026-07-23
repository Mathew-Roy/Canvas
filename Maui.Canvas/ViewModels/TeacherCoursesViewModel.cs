using System.Collections.Generic;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace Maui.Canvas.ViewModels
{
    public class TeacherCoursesViewModel
    {
        public List<Course> Courses { get; }

        public TeacherCoursesViewModel()
        {
            Courses = CourseServiceProxy.Current.Courses;
        }
    }
}