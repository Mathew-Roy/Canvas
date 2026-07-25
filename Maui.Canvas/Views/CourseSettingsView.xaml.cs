using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class CourseSettingsView : ContentPage
{
    private int _courseId;
    private Course? _course;

    public string CourseId
    {
        set
        {
            int.TryParse(value, out _courseId);
            _course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            if (_course != null)
            {
                EntryA.Text = _course.GradeA.ToString();
                EntryB.Text = _course.GradeB.ToString();
                EntryC.Text = _course.GradeC.ToString();
                EntryD.Text = _course.GradeD.ToString();
            }
        }
    }

    public CourseSettingsView()
    {
        InitializeComponent();
    }

    private async void OnSave(object sender, EventArgs e)
    {
        if (_course == null) return;
        if (int.TryParse(EntryA.Text, out int a)) _course.GradeA = a;
        if (int.TryParse(EntryB.Text, out int b)) _course.GradeB = b;
        if (int.TryParse(EntryC.Text, out int c)) _course.GradeC = c;
        if (int.TryParse(EntryD.Text, out int d)) _course.GradeD = d;
        await DisplayAlert("Saved", "Grade ranges updated.", "OK");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}