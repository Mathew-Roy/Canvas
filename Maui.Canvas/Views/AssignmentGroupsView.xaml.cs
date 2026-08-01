using System;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class AssignmentGroupsView : ContentPage
{
    private int _courseId;

    public string CourseId
    {
        set { int.TryParse(value, out _courseId); Reload(); }
    }

    public AssignmentGroupsView()
    {
        InitializeComponent();
    }

    private void Reload()
    {
        if (_courseId > 0)
        {
            var vm = new AssignmentGroupsViewModel();
            vm.Load(_courseId);
            BindingContext = vm;
        }
    }

    private void OnAddGroup(object sender, EventArgs e)
    {
        var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
        if (course == null) return;

        string name = NewGroupName.Text;
        if (string.IsNullOrWhiteSpace(name)) return;

        int newId = course.AssignmentGroups.Any() ? course.AssignmentGroups.Max(g => g.Id) + 1 : 1;
        course.AssignmentGroups.Add(new AssignmentGroup { Id = newId, Name = name, Weight = 0 });
        NewGroupName.Text = string.Empty;
        Reload();
    }

    private async void OnSaveWeight(object sender, EventArgs e)
    {
        if (((Button)sender).BindingContext is GroupRow row)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var group = course?.AssignmentGroups.FirstOrDefault(g => g.Id == row.GroupId);
            if (group == null) return;

            if (double.TryParse(row.WeightText, out double w))
            {
                group.Weight = w;
                await DisplayAlert("Saved", $"'{group.Name}' weight set to {w}%.", "OK");
            }
        }
    }

    private async void OnAssign(object sender, EventArgs e)
    {
        if (AssignmentPicker.SelectedItem is Assignment assignment && GroupPicker.SelectedItem is GroupRow row)
        {
            assignment.GroupId = row.GroupId;
            await DisplayAlert("Assigned", $"'{assignment.Name}' added to group '{row.Name}'.", "OK");
        }
        else
        {
            await DisplayAlert("Pick both", "Select an assignment and a group first.", "OK");
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