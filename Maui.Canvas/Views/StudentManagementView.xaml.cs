using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

public partial class StudentManagementView : ContentPage
{
    public StudentManagementView()
    {
        InitializeComponent();
        Reload();
    }

    private void Reload()
    {
        var vm = new StudentManagementViewModel();
        vm.Load();
        BindingContext = vm;
    }

    private void OnAddStudent(object sender, EventArgs e)
    {
        string name = NewStudentName.Text;
        if (string.IsNullOrWhiteSpace(name)) return;

        var students = StudentServiceProxy.Current.Students;
        int newId = students.Any() ? students.Max(s => s.Id) + 1 : 1;

        students.Add(new Student
        {
            Id = newId,
            Name = name,
            Classification = NewStudentClassification.Text ?? ""
        });

        NewStudentName.Text = string.Empty;
        NewStudentClassification.Text = string.Empty;
        Reload();
    }

    private async void OnEditStudent(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is Student student)
        {
            var target = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == student.Id);
            if (target == null) return;

            string name = await DisplayPromptAsync("Edit Student", "Name:",
                initialValue: target.Name ?? "");
            if (!string.IsNullOrWhiteSpace(name)) target.Name = name;

            string classification = await DisplayPromptAsync("Edit Student", "Classification:",
                initialValue: target.Classification ?? "");
            if (!string.IsNullOrWhiteSpace(classification)) target.Classification = classification;

            Reload();
        }
    }

    private async void OnRemoveStudent(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is Student student)
        {
            bool confirm = await DisplayAlert("Remove Student",
                $"Remove {student.Name}? This deletes their enrollments, submissions, and grades.",
                "Remove", "Cancel");
            if (!confirm) return;

            // Cascade: remove from every course roster and delete their submissions
            foreach (var course in CourseServiceProxy.Current.Courses)
            {
                course.Roster.RemoveAll(s => s.Id == student.Id);
                foreach (var assignment in course.Assignments)
                    assignment.Submissions.RemoveAll(sub => sub.StudentId == student.Id);
            }

            StudentServiceProxy.Current.Students.RemoveAll(s => s.Id == student.Id);
            Reload();
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}