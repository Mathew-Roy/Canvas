using System.Collections.Generic;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace Maui.Canvas.ViewModels
{
    public class SemesterManagementViewModel
    {
        public List<Semester> Semesters { get; private set; } = new();

        public void Load()
        {
            Semesters = SemesterServiceProxy.Current.Semesters.ToList();
        }
    }
}