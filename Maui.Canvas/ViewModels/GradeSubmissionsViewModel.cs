using System.Collections.Generic;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace Maui.Canvas.ViewModels
{
    public class GradeRow
    {
        public int AssignmentId { get; set; }
        public int SubmissionId { get; set; }
        public string Header { get; set; } = "";
        public string Content { get; set; } = "";
        public int AvailablePoints { get; set; }
        public string GradeText { get; set; } = "";
        public string FeedbackText { get; set; } = "";
    }

    public class GradeSubmissionsViewModel
    {
        public string CourseTitle { get; private set; } = "Grade Submissions";
        public List<GradeRow> Rows { get; private set; } = new();
        public bool HasNone => Rows.Count == 0;

        public void Load(int courseId)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return;

            CourseTitle = $"Grade \u2014 {course.Name}";
            var students = StudentServiceProxy.Current.Students;

            var rows = new List<GradeRow>();
            foreach (var a in course.Assignments)
            {
                foreach (var s in a.Submissions)
                {
                    var student = students.FirstOrDefault(st => st.Id == s.StudentId);
                    rows.Add(new GradeRow
                    {
                        AssignmentId = a.Id,
                        SubmissionId = s.Id,
                        Header = $"{a.Name} \u2014 {student?.Name ?? ("Student " + s.StudentId)}",
                        Content = s.Content ?? "",
                        AvailablePoints = a.AvailablePoints,
                        GradeText = s.Grade?.ToString() ?? "",
                        FeedbackText = s.Feedback ?? ""
                    });
                }
            }
            Rows = rows;
        }
    }
}