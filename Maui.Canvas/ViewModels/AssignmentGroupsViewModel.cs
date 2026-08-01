using System.Collections.Generic;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace Maui.Canvas.ViewModels
{
    public class GroupRow
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = "";
        public string WeightText { get; set; } = "";
    }

    public class AssignmentGroupsViewModel
    {
        public string CourseTitle { get; private set; } = "Assignment Groups";
        public List<GroupRow> Groups { get; private set; } = new();
        public List<Assignment> Assignments { get; private set; } = new();

        public void Load(int courseId)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return;

            CourseTitle = $"Groups \u2014 {course.Name}";
            Groups = course.AssignmentGroups
                .Select(g => new GroupRow { GroupId = g.Id, Name = g.Name ?? "", WeightText = g.Weight.ToString() })
                .ToList();
            Assignments = course.Assignments.ToList();
        }
    }
}