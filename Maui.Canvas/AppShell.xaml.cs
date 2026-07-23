using Maui.Canvas.Views;

namespace Maui.Canvas;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("teacher", typeof(TeacherView));
        Routing.RegisterRoute("student", typeof(StudentView));
        Routing.RegisterRoute("studentcourses", typeof(StudentCoursesView));
        Routing.RegisterRoute("coursedetail", typeof(CourseDetailView));
    }
}