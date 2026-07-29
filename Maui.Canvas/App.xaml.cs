using Library.Canvas.Services;

namespace Maui.Canvas;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());
        window.Destroying += (s, e) =>
        {
            StudentServiceProxy.Current.Save();
            CourseServiceProxy.Current.Save();
        };
        return window;
    }
}