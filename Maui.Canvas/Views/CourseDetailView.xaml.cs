using System;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;
using Microsoft.Maui.Storage;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(StudentId), "studentId")]
public partial class CourseDetailView : ContentPage
{
    private int _courseId;
    private int _studentId;

    public string CourseId { set { int.TryParse(value, out _courseId); TryLoad(); } }
    public string StudentId { set { int.TryParse(value, out _studentId); TryLoad(); } }

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

    private void OnChoiceSelected(object sender, EventArgs e)
    {
        if (sender is Picker picker
            && picker.BindingContext is AssignmentDisplay display
            && picker.SelectedItem is string choice)
        {
            display.ResponseText = choice;
        }
    }

    private async void OnAttachFile(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is AssignmentDisplay display)
        {
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                display.AttachedFileName = result.FileName;
                display.AttachedFilePath = result.FullPath;
                await DisplayAlert("File Attached", $"Attached: {result.FileName}", "OK");
            }
        }
    }

    private async void OnSubmitResponse(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is AssignmentDisplay display)
        {
            if (string.IsNullOrWhiteSpace(display.ResponseText)
                && string.IsNullOrWhiteSpace(display.AttachedFileName))
            {
                await DisplayAlert("Empty", "Type a response or attach a file before submitting.", "OK");
                return;
            }

            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var assignment = course?.Assignments.FirstOrDefault(a => a.Id == display.AssignmentId);

            if (assignment != null)
            {
                assignment.Submissions.Add(new Submission
                {
                    Id = assignment.Submissions.Count + 1,
                    StudentId = _studentId,
                    AssignmentId = assignment.Id,
                    Content = display.ResponseText,
                    AttachedFileName = display.AttachedFileName,
                    AttachedFilePath = display.AttachedFilePath,
                    SubmissionDate = DateTime.Now
                });

                await DisplayAlert("Submitted",
                    $"Your response to '{assignment.Name}' was submitted." +
                    (string.IsNullOrWhiteSpace(display.AttachedFileName) ? "" : $"\nFile: {display.AttachedFileName}"),
                    "OK");
            }
        }
    }

    private async void OnAddComment(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is AssignmentDisplay display)
        {
            string text = await DisplayPromptAsync("Add Comment", "Your comment:");
            if (string.IsNullOrWhiteSpace(text)) return;

            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var assignment = course?.Assignments.FirstOrDefault(a => a.Id == display.AssignmentId);
            var student = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == _studentId);
            if (assignment != null)
            {
                assignment.Comments.Add(new Comment
                {
                    Author = student?.Name ?? "Student",
                    Text = text,
                    PostedAt = DateTime.Now
                });
                TryLoad();
            }
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