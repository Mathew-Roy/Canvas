using System.Collections.Generic;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace Maui.Canvas.ViewModels
{
    public class StudentManagementViewModel
    {
        public List<Student> Students { get; private set; } = new();

        public void Load()
        {
            Students = StudentServiceProxy.Current.Students.ToList();
        }
    }
}