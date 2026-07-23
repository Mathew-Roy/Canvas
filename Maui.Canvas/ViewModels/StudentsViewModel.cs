using System.Collections.Generic;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace Maui.Canvas.ViewModels
{
    public class StudentsViewModel
    {
        public List<Student> Students { get; }

        public StudentsViewModel()
        {
            Students = StudentServiceProxy.Current.Students;
        }
    }
}