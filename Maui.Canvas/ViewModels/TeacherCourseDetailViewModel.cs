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
        public List<Assignment> Assignments { get; private set; } = new();
        public List<ModuleRow> ModuleRows { get; private set; } = new();
        public List<Student> AvailableToAdd { get; private set; } = new();

        public void Load(int courseId)
        {
            Course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
            if (Course == null) return;

            Roster = Course.Roster.ToList();
            AvailableToAdd = StudentServiceProxy.Current.Students
                .Where(s => Course.Roster.All(r => r.Id != s.Id))
                .ToList();
            Assignments = Course.Assignments.ToList();
            ModuleRows = Course.Modules.Select(m => new ModuleRow
            {
                ModuleId = m.Id,
                Title = $"Module {m.Id}",
                Items = m.Content.Select((item, idx) => new ModuleItemRow
                {
                    ModuleId = m.Id,
                    ItemIndex = idx,
                    Text = item.Display()
                }).ToList()
            }).ToList();
        }
    }

    public class ModuleItemRow
    {
        public int ModuleId { get; set; }
        public int ItemIndex { get; set; }
        public string Text { get; set; } = "";
    }

    public class ModuleRow
    {
        public int ModuleId { get; set; }
        public string Title { get; set; } = "";
        public List<ModuleItemRow> Items { get; set; } = new();
    }
}
