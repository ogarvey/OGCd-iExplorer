using OGLibCDi.Models;
using ReactiveUI;

namespace OGCdiExplorer.ViewModels.Pages;

public class OverviewViewModel : PageViewModel
{
    public static string Title => "Overview";
    public override bool CanNavigateNext { get; protected set; }
    public override bool CanNavigatePrevious { get; protected set; }
}