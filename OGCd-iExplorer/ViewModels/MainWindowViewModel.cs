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
        CurrentPage = 
            new AnalysisViewModel();
    }

    private PageViewModel? _currentPage;

    private readonly PageViewModel[] _pages =
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