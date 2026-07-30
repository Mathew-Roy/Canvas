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
        public string? AttachedFileName { get; set; }
        public string? AttachedFilePath { get; set; }
        public bool IsQuiz { get; set; }
        public string? Question { get; set; }
        public List<string> CommentLines { get; set; } = new();
        public List<string> Choices { get; set; } = new();
        public bool IsMultipleChoice => IsQuiz && Choices.Count > 0;
        public bool IsFreeResponse => !IsMultipleChoice;
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
        public string LetterColor { get; private set; } = "#FFFFFF";
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
                var sub = a.Submissions.FirstOrDefault(s => s.StudentId == studentId && s.Grade.HasValue);
                string grade = sub != null ? $"Your grade: {sub.Grade}/{a.AvailablePoints}" : "Not graded";
                return new AssignmentDisplay
                {
                    AssignmentId = a.Id,
                    Name = a.Name ?? "",
                    Info = $"{a.AvailablePoints} pts \u2022 due {a.DueDate:MM/dd/yyyy}",
                    GradeText = grade,
                    IsQuiz = a.IsQuiz,
                    Question = a.Question,
                    CommentLines = a.Comments.Select(x => $"{x.Author}: {x.Text}").ToList(),
                    Choices = a.Choices
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
                    var sub = a.Submissions.FirstOrDefault(s => s.StudentId == studentId && s.Grade.HasValue);
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

            LetterColor = LetterGrade switch
            {
                "A" => Course.ColorA,
                "B" => Course.ColorB,
                "C" => Course.ColorC,
                "D" => Course.ColorD,
                "F" => Course.ColorF,
                _ => "#FFFFFF"
            };
        }

        private string ToLetter(double pct)
        {
            if (Course == null) return "F";
            if (pct >= Course.GradeA) return "A";
            if (pct >= Course.GradeB) return "B";
            if (pct >= Course.GradeC) return "C";
            if (pct >= Course.GradeD) return "D";
            return "F";
        }
    }
}