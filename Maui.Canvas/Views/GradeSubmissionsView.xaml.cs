using System;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class GradeSubmissionsView : ContentPage
{
    private int _courseId;

    public string CourseId
    {
        set { int.TryParse(value, out _courseId); Reload(); }
    }

    public GradeSubmissionsView()
    {
        InitializeComponent();
    }

    private void Reload()
    {
        if (_courseId > 0)
        {
            var vm = new GradeSubmissionsViewModel();
            vm.Load(_courseId);
            BindingContext = vm;
        }
    }

    private async void OnSaveGrade(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is GradeRow row)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var assignment = course?.Assignments.FirstOrDefault(a => a.Id == row.AssignmentId);
            var sub = assignment?.Submissions.FirstOrDefault(s => s.Id == row.SubmissionId);
            if (sub == null || assignment == null) return;

            if (double.TryParse(row.GradeText, out double g))
            {
                if (g < 0 || g > assignment.AvailablePoints)
                {
                    await DisplayAlert("Invalid", $"Grade must be between 0 and {assignment.AvailablePoints}.", "OK");
                    return;
                }
                sub.Grade = g;
            }
            sub.Feedback = row.FeedbackText;

            await DisplayAlert("Saved", "Grade saved.", "OK");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnMainMenuClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}