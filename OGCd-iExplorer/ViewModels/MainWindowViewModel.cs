using System;
using System.Collections.ObjectModel;
using OGCdiExplorer.Services;
using OGCdiExplorer.ViewModels.Pages;
using OGCdiExplorer.ViewModels.Windows;
using OGCdiExplorer.Views.Windows;
using OGLibCDi.Models;
using ReactiveUI;

namespace OGCdiExplorer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private BoltFileParser? _boltFileParser;
    private PaletteManagementView? _paletteManagementView;
    
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
        if (_paletteManagementView is not null)
        {
            _paletteManagementView.Activate();
        }
        
        var viewModel = new PaletteManagementViewModel();
        _paletteManagementView = new PaletteManagementView
        {
            DataContext = viewModel
        };
        _paletteManagementView.Closed += (sender, args) =>
        {
            _paletteManagementView = null;
        };
        _paletteManagementView.Show();
    }
    
    public void OpenBoltFileParser()
    {
        
        if (_boltFileParser is not null)
        {
            _boltFileParser.Activate();
        }
        var viewModel = new BoltFileParserViewModel();
        _boltFileParser = new BoltFileParser
        {
            DataContext = viewModel
        };
        _boltFileParser.Closed += (sender, args) =>
        {
            _boltFileParser = null;
        };
        _boltFileParser.Show();
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