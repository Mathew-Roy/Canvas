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
        public int AssignmentId { get; set; }
        public string ResponseText { get; set; } = "";
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
        public string LetterGrade { get; private set; } = "N/A";
        public string GradePercentText { get; private set; } = "";
        public List<AssignmentDisplay> Assignments { get; private set; } = new();
        public List<ModuleDisplay> Modules { get; private set; } = new();
        public List<string> Announcements { get; private set; } = new();
        public bool HasAnnouncements => Announcements.Count > 0;
        public void Load(int courseId, int studentId)
        {
            Course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
            if (Course == null) return;
            Announcements = Course.Announcements.ToList();
            Assignments = Course.Assignments.Select(a =>
            {
                var sub = a.Submissions
                    .FirstOrDefault(s => s.StudentId == studentId && s.Grade.HasValue);
                string grade = sub != null
                    ? $"Your grade: {sub.Grade}/{a.AvailablePoints}"
                    : "Not graded";
                return new AssignmentDisplay
                {
                    AssignmentId = a.Id,
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

            double total = 0;
            bool anyGraded = false;
            foreach (var group in Course.AssignmentGroups)
            {
                var percents = new List<double>();
                foreach (var a in Course.Assignments.Where(a => a.GroupId == group.Id))
                {
                    var sub = a.Submissions
                        .FirstOrDefault(s => s.StudentId == studentId && s.Grade.HasValue);
                    if (sub != null && a.AvailablePoints > 0)
                    {
                        percents.Add(sub.Grade!.Value / a.AvailablePoints * 100.0);
                        anyGraded = true;
                    }
                }
                if (percents.Count > 0)
                    total += percents.Average() * (group.Weight / 100.0);
            }

            if (anyGraded)
            {
                LetterGrade = ToLetter(total);
                GradePercentText = $"{total:F1}%";
            }
            else
            {
                LetterGrade = "N/A";
                GradePercentText = "No graded work yet";
            }
        }
        private static string ToLetter(double pct)
        {
            if (pct >= 90) return "A";
            if (pct >= 80) return "B";
            if (pct >= 70) return "C";
            if (pct >= 60) return "D";
            return "F";
        }
        
    }
}