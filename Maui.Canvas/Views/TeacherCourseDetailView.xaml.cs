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

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}