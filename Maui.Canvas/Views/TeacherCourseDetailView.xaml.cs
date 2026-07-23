using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherCourseDetailView : ContentPage
{
    private int _courseId;

    public string CourseId
    {
        set { int.TryParse(value, out _courseId); Reload(); }
    }

    public TeacherCourseDetailView()
    {
        InitializeComponent();
    }

    private void Reload()
    {
        if (_courseId > 0)
        {
            var vm = new TeacherCourseDetailViewModel();
            vm.Load(_courseId);
            BindingContext = vm;
        }
    }

    private void OnAddClicked(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is Student student)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            if (course != null && course.Roster.All(r => r.Id != student.Id))
                course.Roster.Add(student);
            Reload();
        }
    }

    private void OnRemoveClicked(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is Student student)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var inRoster = course?.Roster.FirstOrDefault(r => r.Id == student.Id);
            if (inRoster != null)
                course!.Roster.Remove(inRoster);
            Reload();
        }
    }

    private void OnAddAssignment(object sender, EventArgs e)
    {
        var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
        if (course == null) return;

        string name = NewAssignmentName.Text;
        if (string.IsNullOrWhiteSpace(name)) return;

        int.TryParse(NewAssignmentPoints.Text, out int points);
        int newId = course.Assignments.Any() ? course.Assignments.Max(a => a.Id) + 1 : 1;

        course.Assignments.Add(new Assignment
        {
            Id = newId,
            Name = name,
            AvailablePoints = points,
            DueDate = NewAssignmentDue.Date
        });

        NewAssignmentName.Text = string.Empty;
        NewAssignmentPoints.Text = string.Empty;
        Reload();
    }

    private void OnDeleteAssignment(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is Assignment assignment)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var target = course?.Assignments.FirstOrDefault(a => a.Id == assignment.Id);
            if (target != null)
            {
                target.Submissions.Clear();          // cascade: delete its submissions
                course!.Assignments.Remove(target);
            }
            Reload();
        }
    }

    private async void OnEditAssignment(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is Assignment assignment)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var target = course?.Assignments.FirstOrDefault(a => a.Id == assignment.Id);
            if (target == null) return;

            string newName = await DisplayPromptAsync("Edit Assignment", "Name:",
                initialValue: target.Name ?? "");
            if (!string.IsNullOrWhiteSpace(newName))
                target.Name = newName;

            string newPoints = await DisplayPromptAsync("Edit Assignment", "Available points:",
                initialValue: target.AvailablePoints.ToString());
            if (int.TryParse(newPoints, out int pts))
                target.AvailablePoints = pts;

            Reload();
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}