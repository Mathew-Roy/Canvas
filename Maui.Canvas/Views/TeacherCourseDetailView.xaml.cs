using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;
using Microsoft.Maui.Storage;

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

    private void OnAddModule(object sender, EventArgs e)
    {
        var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
        if (course == null) return;

        int newId = course.Modules.Any() ? course.Modules.Max(m => m.Id) + 1 : 1;
        course.Modules.Add(new Module { Id = newId });
        Reload();
    }

    private void OnDeleteModule(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is ModuleRow row)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var mod = course?.Modules.FirstOrDefault(m => m.Id == row.ModuleId);
            if (mod != null) course!.Modules.Remove(mod);
            Reload();
        }
    }

    private async void OnAddPage(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is ModuleRow row)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var mod = course?.Modules.FirstOrDefault(m => m.Id == row.ModuleId);
            if (mod == null) return;

            string text = await DisplayPromptAsync("Add Page", "Page content:");
            if (!string.IsNullOrWhiteSpace(text))
                mod.Content.Add(new PageItem { Content = text });
            Reload();
        }
    }

    private void OnRemoveItem(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is ModuleItemRow row)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var mod = course?.Modules.FirstOrDefault(m => m.Id == row.ModuleId);
            if (mod != null && row.ItemIndex >= 0 && row.ItemIndex < mod.Content.Count)
                mod.Content.RemoveAt(row.ItemIndex);
            Reload();
        }
    }

    private async void OnEditItem(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is ModuleItemRow row)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var mod = course?.Modules.FirstOrDefault(m => m.Id == row.ModuleId);
            if (mod == null || row.ItemIndex < 0 || row.ItemIndex >= mod.Content.Count) return;

            if (mod.Content[row.ItemIndex] is PageItem page)
            {
                string text = await DisplayPromptAsync("Edit Page", "Page content:",
                    initialValue: page.Content ?? "");
                if (!string.IsNullOrWhiteSpace(text))
                    page.Content = text;
            }
            else
            {
                await DisplayAlert("Not editable", "Only page content can be edited here.", "OK");
            }
            Reload();
        }
    }
    private void OnCopyAssignments(object sender, EventArgs e)
    {
        if (SourceCoursePicker.SelectedItem is not Course source) return;

        var dest = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
        if (dest == null) return;

        foreach (var a in source.Assignments)
        {
            int newId = dest.Assignments.Any() ? dest.Assignments.Max(x => x.Id) + 1 : 1;
            dest.Assignments.Add(new Assignment
            {
                Id = newId,
                Name = a.Name,
                Description = a.Description,
                AvailablePoints = a.AvailablePoints,
                DueDate = a.DueDate,
                GroupId = a.GroupId
                // Submissions intentionally NOT copied
            });
        }

        Reload();
    }
    private async void OnExportRoster(object sender, EventArgs e)
    {
        var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
        if (course == null) return;

        var lines = course.Roster.Select(s => $"{s.Id},{s.Name}");
        string path = System.IO.Path.Combine(FileSystem.AppDataDirectory, "roster_export.csv");
        System.IO.File.WriteAllLines(path, lines);

        await DisplayAlert("Exported",
            $"Exported {course.Roster.Count} student(s) to:\n{path}", "OK");
    }

    private async void OnImportRoster(object sender, EventArgs e)
    {
        var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
        if (course == null) return;

        string path = System.IO.Path.Combine(FileSystem.AppDataDirectory, "roster_export.csv");
        if (!System.IO.File.Exists(path))
        {
            await DisplayAlert("No File", "No roster file found. Export one first.", "OK");
            return;
        }

        int added = 0;
        foreach (var line in System.IO.File.ReadAllLines(path))
        {
            var parts = line.Split(',');
            if (parts.Length < 1 || !int.TryParse(parts[0], out int id)) continue;

            // idempotent + non-destructive: only add students not already enrolled
            if (course.Roster.Any(s => s.Id == id)) continue;

            var student = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                course.Roster.Add(student);
                added++;
            }
        }

        await DisplayAlert("Imported", $"Added {added} new student(s).", "OK");
        Reload();
    }
    private void OnPostAnnouncement(object sender, EventArgs e)
    {
        var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
        if (course == null) return;

        string text = NewAnnouncement.Text;
        if (string.IsNullOrWhiteSpace(text)) return;

        course.Announcements.Add(text);
        NewAnnouncement.Text = string.Empty;
        Reload();
    }

    private void OnDeleteAnnouncement(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is string text)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            course?.Announcements.Remove(text);
            Reload();
        }
    }
}