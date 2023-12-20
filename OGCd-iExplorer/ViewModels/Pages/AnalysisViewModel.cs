using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using OGCdiExplorer.Services;
using OGCdiExplorer.ViewModels.Windows;
using OGCdiExplorer.Views.Pages;
using OGLibCDi.Enums;
using OGLibCDi.Models;
using ReactiveUI;

namespace OGCdiExplorer.ViewModels.Pages;

public class AnalysisViewModel : PageViewModel
{
    public override bool CanNavigateNext { get; protected set; }
    public override bool CanNavigatePrevious { get; protected set; }
    
    public AnalysisViewModel()
    {
        SelectedCdiFile = CdiFileService.Instance.CdiFile;
        FilteredItems = new ObservableCollection<CdiSector>(SelectedCdiFile.Sectors);
        Sectors = new ObservableCollection<CdiSector>(SelectedCdiFile.Sectors);
        
        ShowDialog = new Interaction<ImagePreviewViewModel, bool?>();

        ImagePreviewCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new ImagePreviewViewModel(_videoType);

            var result = await ShowDialog.Handle(store);
        });
        
    }

    public ICommand ImagePreviewCommand { get; }

    public Interaction<ImagePreviewViewModel, bool?> ShowDialog { get; }
    
    private ObservableCollection<CdiSector>? _sectors;

    public ObservableCollection<CdiSector>? Sectors { 
        get => _sectors;
        set => this.RaiseAndSetIfChanged(ref _sectors, value);
    } 
    
    private ObservableCollection<CdiSector>? _filteredItems;

    public ObservableCollection<CdiSector>? FilteredItems
    {
        get => _filteredItems;
        set => this.RaiseAndSetIfChanged(ref _filteredItems, value);
    }
    
    private bool _filterAudio = false;
    public bool FilterAudio
    {
        get => _filterAudio;
        set => this.RaiseAndSetIfChanged(ref _filterAudio, value);
    }

    private bool _filterVideo = false;
    public bool FilterVideo
    {
        get => _filterVideo;
        set => this.RaiseAndSetIfChanged(ref _filterVideo, value);
    }

    private bool _filterData = false;
    private CdiVideoType _videoType;
    public CdiVideoType VideoType
    {
        get => _videoType;
        set => this.RaiseAndSetIfChanged(ref _videoType, value);
    }
    public bool FilterData
    {
        get => _filterData;
        set => this.RaiseAndSetIfChanged(ref _filterData, value);
    }

    private int _filterChannel = -1;
    public int FilterChannel
    {
        get => _filterChannel;
        set => this.RaiseAndSetIfChanged(ref _filterChannel, value);
    }
    
    private int _filterVideoType = -1;
    public int FilterVideoType
    {
        get => _filterVideoType;
        set => this.RaiseAndSetIfChanged(ref _filterVideoType, value);
    }
    
    public void ApplySectorTypeFilters()
    {
        var filtered = _sectors?.Where(item =>
            (!FilterAudio || item.SectorTypeString == "Audio") ||
            (!FilterVideo || item.SectorTypeString == "Video") ||
            (!FilterData || item.SectorTypeString == "Data")).OrderBy(s => s.SectorIndex).ToList();
        
        filtered = filtered?.Where(item => (FilterChannel != -1 && item.Channel == FilterChannel) || (FilterChannel == -1)).ToList();
        filtered = filtered?.Where(item => (FilterVideoType != -1 && item.Coding.Coding == FilterVideoType) || (FilterVideoType == -1)).ToList();
        
        if (filtered != null) FilteredItems = new ObservableCollection<CdiSector>(filtered);
    }
    
}