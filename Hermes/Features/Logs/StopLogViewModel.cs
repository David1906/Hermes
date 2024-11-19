using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hermes.Common.Extensions;
using Hermes.Common;
using Hermes.Models;
using Hermes.Repositories;
using Hermes.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Hermes.Features.Logs;

public partial class StopLogViewModel : ViewModelBase
{
    [ObservableProperty] private string _serialNumberFilter = "";
    [ObservableProperty] private string _detailsFilter = "";
    [ObservableProperty] private StopType? _selectedStopType;
    [ObservableProperty] private TimeSpanType? _selectedTimeSpan;

    public RangeObservableCollection<UnitUnderTest> UnitsUnderTest { get; set; } = [];
    public static IEnumerable<StopType?> StopTypeOptions => NullableExtensions.GetValues<StopType>();
    public static IEnumerable<TimeSpanType?> TimeSpanOptions => NullableExtensions.GetValues<TimeSpanType>();

    private readonly StopRepository _stopRepository;


    public StopLogViewModel(StopRepository stopRepository)
    {
        _stopRepository = stopRepository;
    }

    [RelayCommand]
    private async Task Refresh()
    {
        SerialNumberFilter = "";
        DetailsFilter = "";
        SelectedStopType = null;
        SelectedTimeSpan = null;
        await Search();
    }

    [RelayCommand]
    private async Task Search()
    {
        var units = await _stopRepository.GetLast(
            SerialNumberFilter,
            DetailsFilter,
            SelectedStopType,
            lastTimeSpan: SelectedTimeSpan == null ? null : TimeSpan.FromHours((int)SelectedTimeSpan));

        UnitsUnderTest.Clear();
        UnitsUnderTest.AddRange(units);
    }
}