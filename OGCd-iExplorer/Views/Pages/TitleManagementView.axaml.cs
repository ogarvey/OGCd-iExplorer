using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OGCdiExplorer.Models;
using OGCdiExplorer.Services;
using OGCdiExplorer.ViewModels.Pages;

namespace OGCdiExplorer.Views.Pages;

public partial class TitleManagementView : UserControl
{
    public TitleManagementView()
    {
        InitializeComponent();
    }
    
    public async void AddTitleCommand(object? sender, RoutedEventArgs e)
    {
        await DatabaseService.Instance.CreateCdiTitle(((TitleManagementViewModel)DataContext).TitleDetail);
        ((TitleManagementViewModel)DataContext).Titles.Add(((TitleManagementViewModel)DataContext).TitleDetail);
        ((TitleManagementViewModel)DataContext).TitleDetail = new CdiTitle();
    }

    private void CreateTitleCommand(object? sender, RoutedEventArgs e)
    {
        ((TitleManagementViewModel)DataContext).IsNewTitle = true;
    }

    private void ExistingTitle_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var title = ExistingTitle.SelectedItem as CdiTitle;
        ((TitleManagementViewModel)DataContext).TitleDetail = title;
        ((TitleManagementViewModel)DataContext).IsTitleSelected = true;
    }
}