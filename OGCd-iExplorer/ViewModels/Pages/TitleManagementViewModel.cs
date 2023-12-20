using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using OGCdiExplorer.Models;
using OGCdiExplorer.Services;
using ReactiveUI;

namespace OGCdiExplorer.ViewModels.Pages;

public class TitleManagementViewModel: PageViewModel
{
    public override bool CanNavigateNext { get; protected set; }
    public override bool CanNavigatePrevious { get; protected set; }
    
    public TitleManagementViewModel()
    {
        Titles = new ObservableCollection<CdiTitle>(DatabaseService.Instance.GetCdiTitles().Result);
    }
    
    private ObservableCollection<CdiTitle>? _titles;

    public ObservableCollection<CdiTitle>? Titles { 
        get => _titles;
        set => this.RaiseAndSetIfChanged(ref _titles, value);
    } 
    
    private CdiTitle? _selectedTitle;
    public CdiTitle SelectedTitle
    {
        get => _selectedTitle;
        set => this.RaiseAndSetIfChanged(ref _selectedTitle, value);
    }
    
    private bool _isNewTitle = false;
    public bool IsNewTitle
    {
        get => _isNewTitle;
        set
        {
            this.RaiseAndSetIfChanged(ref _isNewTitle, value);
            this.RaisePropertyChanged(nameof(ShowTitleDetail));
        }
    }
    
    private bool _isTitleSelected = false;

    public bool IsTitleSelected
    {
        get => _isTitleSelected;
        set
        {
            this.RaiseAndSetIfChanged(ref _isTitleSelected, value);
            this.RaisePropertyChanged(nameof(ShowTitleDetail));
        }
    }

    public bool ShowTitleDetail => IsTitleSelected || IsNewTitle;
    
    private CdiTitle _newTitle = new CdiTitle();

    public CdiTitle NewTitle
    {
        get => _newTitle;
        set
        {
            this.RaiseAndSetIfChanged(ref _newTitle, value);
        }
    }


}