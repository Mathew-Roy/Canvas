using System;
using System.Collections.Generic;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace Maui.Canvas.ViewModels
{
    public class AssignmentDisplay
    {
        public string Name { get; set; } = "";
        public string Info { get; set; } = "";
        public string GradeText { get; set; } = "";
    }

    public class ModuleDisplay
    {
        public string Title { get; set; } = "";
        public List<string> Items { get; set; } = new();
    }

    public class CourseDetailViewModel
    {
        public Course? Course { get; private set; }
        public string CourseTitle => Course != null ? $"{Course.Name} ({Course.Code})" : "Course";
        public List<AssignmentDisplay> Assignments { get; private set; } = new();
        public List<ModuleDisplay> Modules { get; private set; } = new();

        public void Load(int courseId, int studentId)
        {
            Course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
            if (Course == null) return;

            Assignments = Course.Assignments.Select(a =>
            {
                var sub = a.Submissions
                    .FirstOrDefault(s => s.StudentId == studentId && s.Grade.HasValue);
                string grade = sub != null
                    ? $"Your grade: {sub.Grade}/{a.AvailablePoints}"
                    : "Not graded";
                return new AssignmentDisplay
                {
                    Name = a.Name ?? "",
                    Info = $"{a.AvailablePoints} pts \u2022 due {a.DueDate:MM/dd/yyyy}",
                    GradeText = grade
                };
            }).ToList();

            Modules = Course.Modules.Select(m => new ModuleDisplay
            {
                Title = $"Module {m.Id}",
                Items = m.Content.Select(item => item.Display()).ToList()
            }).ToList();
        }
    }
}