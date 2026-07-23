using System;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(StudentId), "studentId")]
public partial class CourseDetailView : ContentPage
{
    private int _courseId;
    private int _studentId;

    public string CourseId
    {
        set { int.TryParse(value, out _courseId); TryLoad(); }
    }

    public string StudentId
    {
        set { int.TryParse(value, out _studentId); TryLoad(); }
    }

    public CourseDetailView()
    {
        InitializeComponent();
    }

    private void TryLoad()
    {
        if (_courseId > 0 && _studentId > 0)
        {
            var vm = new CourseDetailViewModel();
            vm.Load(_courseId, _studentId);
            BindingContext = vm;
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnSubmitResponse(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is AssignmentDisplay display)
        {
            if (string.IsNullOrWhiteSpace(display.ResponseText))
            {
                await DisplayAlert("Empty", "Please type a response before submitting.", "OK");
                return;
            }

            var course = CourseServiceProxy.Current.Courses
                .FirstOrDefault(c => c.Id == _courseId);
            var assignment = course?.Assignments
                .FirstOrDefault(a => a.Id == display.AssignmentId);

            if (assignment != null)
            {
                assignment.Submissions.Add(new Submission
                {
                    Id = assignment.Submissions.Count + 1,
                    StudentId = _studentId,
                    AssignmentId = assignment.Id,
                    Content = display.ResponseText,
                    SubmissionDate = DateTime.Now
                });

                await DisplayAlert("Submitted",
                    $"Your response to '{assignment.Name}' was submitted.", "OK");
            }
        }
    }
}