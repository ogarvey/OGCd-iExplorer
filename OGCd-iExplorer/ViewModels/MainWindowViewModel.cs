using System;
using System.Collections.ObjectModel;
using OGCdiExplorer.Services;
using OGCdiExplorer.ViewModels.Pages;
using OGLibCDi.Models;
using ReactiveUI;

namespace OGCdiExplorer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
    }

    public void ViewCdiFile()
    {
        if (_previousPage is AnalysisViewModel)
        {
            (PreviousPage, CurrentPage) = (CurrentPage, PreviousPage);
        }
        else
        {
            PreviousPage = CurrentPage;
            CurrentPage =
                new AnalysisViewModel();
        }
    }

    public void ViewTitleManagement()
    { 
        if (_previousPage is TitleManagementViewModel)
        {
            (PreviousPage, CurrentPage) = (CurrentPage, PreviousPage);
        }
        else
        {
            PreviousPage = CurrentPage;
            CurrentPage =
                new TitleManagementViewModel();
        }
    }

    public void ViewPaletteManagement()
    { 
        if (_previousPage is PaletteManagementViewModel)
        {
            (PreviousPage, CurrentPage) = (CurrentPage, PreviousPage);
        }
        else
        {
            PreviousPage = CurrentPage;
            CurrentPage =
                new PaletteManagementViewModel();
        }
    }

    private PageViewModel? _previousPage;

    public PageViewModel PreviousPage
    {
        get { return _previousPage; }
        private set { this.RaiseAndSetIfChanged(ref _previousPage, value); }
    }

    private PageViewModel? _currentPage;

    private readonly PageViewModel[]
        _pages =
        {
            new OverviewViewModel(),
        };

    /// <summary>
    /// Gets the current page. The property is read-only
    /// </summary>
    public PageViewModel CurrentPage
    {
        get { return _currentPage; }
        private set { this.RaiseAndSetIfChanged(ref _currentPage, value); }
    }
}