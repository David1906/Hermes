using CommunityToolkit.Mvvm.ComponentModel;
using ConfigFactory.Core.Attributes;
using Hermes.Common.Extensions;
using Hermes.Common;
using Hermes.Repositories;
using Hermes.Types;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;

#pragma warning disable CS0657 // Not a valid attribute location for this declaration

namespace Hermes.Features.SettingsConfig;

public partial class SettingsConfigModel(
    ILogger logger,
    ISettingsRepository settingsRepository)
    : BaseConfigModel<SettingsConfigModel>(logger, settingsRepository)
{
    #region General

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "Languages")]
    [property: Config(
        Header = "c_settings_header_language",
        Description = "c_settings_description_language",
        Category = "c_settings_category_general",
        Group = "c_settings_group_common")]
    private LanguageType _language = LanguageType.En;

    public static LanguageType[] Languages => EnumExtensions.GetValues<LanguageType>();

    [ObservableProperty]
    [property: NumericConfig(Minimum = 100, Maximum = 2000, Increment = 1)]
    [property: Config(
        Header = "c_settings_header_wait_delay_ms",
        Description = "c_settings_description_wait_delay_ms",
        Category = "c_settings_category_general",
        Group = "c_settings_group_common")]
    private int _waitDelayMilliseconds = 100;

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_database_server",
        Description = "c_settings_header_database_server",
        Category = "c_settings_category_general",
        Group = "c_settings_group_common")]
    private string _databaseServer = "10.12.204.254";

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_update_manager_url",
        Description = "c_settings_header_update_manager_url",
        Category = "c_settings_category_general",
        Group = "c_settings_group_common")]
    private string _updateManagerUrl = @"http://10.12.204.254/hermes/download";

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_auto_sync_database",
        Description = "c_settings_description_auto_sync_database",
        Category = "c_settings_category_general",
        Group = "c_settings_group_common")]
    private bool _autoSyncDatabase = true;

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "LineType")]
    [property: Config(
        Header = "c_settings_header_line_name",
        Description = "c_settings_header_line_name",
        Category = "c_settings_category_general",
        Group = "c_settings_group_station")]
    private LineType _line = LineType.Ag01;

    public static LineType[] LineTypes => EnumExtensions.GetValues<LineType>();

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "StationType")]
    [property: Config(
        Header = "c_settings_header_station",
        Description = "c_settings_header_station",
        Category = "c_settings_category_general",
        Group = "c_settings_group_station")]
    private StationType _station = StationType.SpiBottom;

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_station_id",
        Description = "c_settings_header_station_id",
        Category = "c_settings_category_general",
        Group = "c_settings_group_station")]
    private string _stationId = "";

    public static StationType[] StationTypes => EnumExtensions.GetValues<StationType>();

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "MachineType")]
    [property: Config(
        Header = "c_settings_header_machine_type",
        Description = "c_settings_header_machine_type",
        Category = "c_settings_category_general",
        Group = "c_settings_group_station")]
    private MachineType _machine = MachineType.Spi;

    public static MachineType[] MachineTypes => EnumExtensions.GetValues<MachineType>();

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "LogfileTypes")]
    [property: Config(
        Header = "c_settings_header_logfile_type",
        Description = "c_settings_header_logfile_type",
        Category = "c_settings_category_general",
        Group = "c_settings_group_station")]
    private LogfileType _logfileType = LogfileType.TriDefault;

    public static LogfileType[] LogfileTypes => EnumExtensions.GetValues<LogfileType>();

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "FileExtensions")]
    [property: Config(
        Header = "c_settings_header_sfc_response_ext",
        Description = "c_settings_header_sfc_response_ext",
        Category = "c_settings_category_general",
        Group = "c_settings_group_station")]
    private FileExtension _sfcResponseExtension = FileExtension.Ret;

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "FileExtensions")]
    [property: Config(
        Header = "c_settings_header_input_file_ext",
        Description = "c_settings_header_input_file_ext",
        Category = "c_settings_category_general",
        Group = "c_settings_group_station")]
    private FileExtension _inputFileExtension = FileExtension.Ret;

    #endregion

    #region UutProcessor

    [ObservableProperty]
    [property: NumericConfig(Minimum = 0, Maximum = 5, Increment = 1)]
    [property: Config(
        Header = "c_settings_header_max_retries",
        Description = "c_settings_description_max_retries",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_common")]
    private int _maxSfcRetries = 1;

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_additional_ok_sfc_response",
        Description = "c_settings_description_additional_ok_sfc_response",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_common")]
    private string _additionalOkSfcResponse = "";

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_scanner_filter",
        Description = "c_settings_description_scanner_filter",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_common")]
    private string _scannerFilter = @"1A\w+$";

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_delete_old_backup_files_on_sunday",
        Description = "c_settings_description_delete_old_backup_files_on_sunday",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_common")]
    private bool _deleteOldBackupFilesOnSunday = true;

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_autostart_uut_processor",
        Description = "c_settings_header_autostart_uut_processor",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_features")]
    private bool _autostartUutProcessor;

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_send_repair_file",
        Description = "c_settings_header_send_repair_file",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_features")]
    private bool _sendRepairFile = true;

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_enable_critical_location_stop",
        Description = "c_settings_description_enable_critical_location_stop",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_features")]
    private bool _enableCriticalLocationStop;

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_critical_locations",
        Description = "c_settings_description_critical_locations",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_features")]
    private string _criticalLocations = "";

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_enable_rule_three_five_ten",
        Description = "c_settings_description_enable_rule_three_five_ten",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_features")]
    private bool _enableRuleThreeFiveTen;

    [ObservableProperty]
    [property: Config(
        Header = "c_settings_header_enable_machine_stop",
        Description = "c_settings_description_enable_machine_stop",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_features")]
    private bool _enableMachineStop;

    [ObservableProperty]
    [property: BrowserConfig(BrowserMode = BrowserMode.OpenFolder, Filter = "Folder:*.*")]
    [property: Config(
        Header = "c_settings_header_input_path",
        Description = "c_settings_header_input_path",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_paths")]
    private string _inputPath = @".\Input";

    [ObservableProperty]
    [property: BrowserConfig(BrowserMode = BrowserMode.OpenFolder, Filter = "Folder:*.*")]
    [property: Config(
        Header = "c_settings_header_backup_path",
        Description = "c_settings_header_backup_path",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_paths")]
    private string _backupPath = @".\Backup";

    [ObservableProperty]
    [property: BrowserConfig(BrowserMode = BrowserMode.OpenFolder, Filter = "Folder:*.*")]
    [property: Config(
        Header = "c_settings_header_sfc_path",
        Description = "c_settings_header_sfc_path",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_paths")]
    private string _sfcPath = @"Z:\";

    public static FileExtension[] FileExtensions => EnumExtensions.GetValues<FileExtension>();

    [ObservableProperty]
    [property: NumericConfig(Minimum = 3, Maximum = 30, Increment = 1)]
    [property: Config(
        Header = "c_settings_header_sfc_timeout",
        Description = "c_settings_header_sfc_timeout",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_time")]
    private int _sfcTimeoutSeconds = 10;

    [ObservableProperty]
    [property: NumericConfig(Minimum = 3, Maximum = 30, Increment = 1)]
    [property: Config(
        Header = "c_settings_header_success_window_timeout",
        Description = "c_settings_description_success_window_timeout",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_time")]
    private int _uutSuccessWindowTimeoutSeconds = 5;

    #region Screen printer

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "GetPortNames")]
    [property: Config(
        Header = "c_settings_header_gkg_tunnel_com_port",
        Description = "c_settings_header_gkg_tunnel_com_port",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_screen_printer")]
    private string _gkgTunnelComPort = "COM50";

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "GetPortNames")]
    [property: Config(
        Header = "c_settings_header_scanner_com_port",
        Description = "c_settings_header_scanner_com_port",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_screen_printer")]
    private string _scannerComPort = "COM40";

    [ObservableProperty]
    [property: NumericConfig(Minimum = 600, Maximum = 115200, Increment = 1)]
    [property: Config(
        Header = "c_settings_header_scanner_com_baud_rate",
        Description = "c_settings_description_scanner_com_baud_rate",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_screen_printer")]
    private int _scannerComBaudRate = 115200;

    [ObservableProperty]
    [property: NumericConfig(Minimum = 7, Maximum = 8, Increment = 1)]
    [property: Config(
        Header = "c_settings_header_scanner_com_data_bits",
        Description = "c_settings_description_scanner_com_data_bits",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_screen_printer")]
    private int _scannerComDataBits = 8;

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "GetParity")]
    [property: Config(
        Header = "c_settings_header_scanner_com_parity",
        Description = "c_settings_description_scanner_com_parity",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_screen_printer")]
    private Parity _scannerComParity = Parity.None;

    public static Parity[] GetParity = [Parity.None, Parity.Even, Parity.Odd, Parity.Mark, Parity.Space];

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "GetStopBits")]
    [property: Config(
        Header = "c_settings_header_scanner_com_stop_bits",
        Description = "c_settings_description_scanner_com_stop_bits",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_screen_printer")]
    private StopBits _scannerComStopBits = StopBits.One;

    public static StopBits[] GetStopBits => [StopBits.One, StopBits.OnePointFive, StopBits.Two];

    private readonly RangeObservableCollection<string> _comPorts = [];

    public RangeObservableCollection<string> GetPortNames()
    {
        return _comPorts;
    }

    #endregion

    #region Labeling machine

    [ObservableProperty]
    [property: DropdownConfig(RuntimeItemsSourceMethodName = "GetPortNames")]
    [property: Config(
        Header = "c_settings_header_overwrite_package_id",
        Description = "c_settings_description_overwrite_package_id",
        Category = "c_settings_category_uut_processor",
        Group = "c_settings_group_labeling_machine")]
    private bool _overwritePackageId = true;

    #endregion

    #endregion

    partial void OnMachineChanged(MachineType value)
    {
        switch (value)
        {
            case MachineType.Axi:
                LogfileType = LogfileType.VitroxDefault;
                SfcResponseExtension = FileExtension.Ret;
                InputFileExtension = FileExtension.Rle;
                SendRepairFile = true;
                EnableMachineStop = true;
                EnableRuleThreeFiveTen = true;
                EnableCriticalLocationStop = true;
                break;
            case MachineType.Spi:
                LogfileType = LogfileType.TriDefault;
                SfcResponseExtension = FileExtension.Log;
                InputFileExtension = FileExtension._3dx;
                SendRepairFile = false;
                EnableMachineStop = true;
                EnableRuleThreeFiveTen = false;
                EnableCriticalLocationStop = false;
                break;
            case MachineType.Aoi:
                LogfileType = LogfileType.TriDefault;
                SfcResponseExtension = FileExtension.Log;
                InputFileExtension = FileExtension._3dx;
                SendRepairFile = true;
                EnableMachineStop = true;
                EnableRuleThreeFiveTen = true;
                EnableCriticalLocationStop = true;
                break;
            case MachineType.Magic:
                LogfileType = LogfileType.MagicDefault;
                SfcResponseExtension = FileExtension.Log;
                InputFileExtension = FileExtension.Txt;
                SendRepairFile = true;
                EnableMachineStop = true;
                EnableRuleThreeFiveTen = true;
                EnableCriticalLocationStop = true;
                break;
            case MachineType.Labeling:
                LogfileType = LogfileType.LabelingMachineDefault;
                SfcResponseExtension = FileExtension.Res;
                InputFileExtension = FileExtension.Txt;
                SendRepairFile = true;
                EnableMachineStop = true;
                EnableRuleThreeFiveTen = false;
                EnableCriticalLocationStop = false;
                break;
            case MachineType.ScreenPrinter:
                LogfileType = LogfileType.GkgDefault;
                SfcResponseExtension = FileExtension.Log;
                InputFileExtension = FileExtension._3dx;
                SendRepairFile = false;
                EnableMachineStop = true;
                EnableRuleThreeFiveTen = false;
                EnableCriticalLocationStop = false;
                OverwritePackageId = true;
                break;
            default:
                LogfileType = LogfileType.Default;
                SfcResponseExtension = FileExtension.Res;
                InputFileExtension = FileExtension.Txt;
                SendRepairFile = true;
                EnableMachineStop = true;
                EnableRuleThreeFiveTen = true;
                EnableCriticalLocationStop = true;
                break;
        }

        this.CalcStationId();
    }

    partial void OnStationChanged(StationType value)
    {
        AdditionalOkSfcResponse = "";
        ScannerFilter = "";
        switch (value)
        {
            case StationType.Labeling:
                Machine = MachineType.None;
                SendRepairFile = false;
                AutostartUutProcessor = false;
                break;
            case StationType.LabelingMachine:
                Machine = MachineType.Labeling;
                SendRepairFile = false;
                AutostartUutProcessor = true;
                AdditionalOkSfcResponse = "GO-S_INPUT_B";
                break;
            case StationType.SpiBottom:
            case StationType.SpiTop:
                Machine = MachineType.Spi;
                SendRepairFile = false;
                AutostartUutProcessor = true;
                break;
            case StationType.Axi:
                Machine = MachineType.Axi;
                SendRepairFile = true;
                AutostartUutProcessor = true;
                break;
            case StationType.Pth:
                Machine = MachineType.Magic;
                SendRepairFile = true;
                AutostartUutProcessor = true;
                break;
            case StationType.ScreenPrinterBottom:
            case StationType.ScreenPrinterTop:
                Machine = MachineType.ScreenPrinter;
                SendRepairFile = false;
                AutostartUutProcessor = true;
                AdditionalOkSfcResponse = value == StationType.ScreenPrinterBottom
                    ? "GO-SPI_B"
                    : "GO-SPI_T";
                ScannerFilter = @"1A\w+$";
                break;
            case StationType.Aoi1:
            case StationType.Aoi2:
            case StationType.Aoi3:
            case StationType.Aoi4:
            default:
                Machine = MachineType.Aoi;
                SendRepairFile = true;
                AutostartUutProcessor = true;
                break;
        }

        this.CalcStationId();
    }

    partial void OnLineChanged(LineType value)
    {
        this.CalcStationId();
    }

    private void CalcStationId()
    {
        var id = Station.GetDescription();
        StationId = !string.IsNullOrEmpty(id) ? $"{(int)Line + 1}1{id}" : string.Empty;
    }

    public void Refresh()
    {
        _comPorts.Clear();
        _comPorts.AddRange(SerialPort.GetPortNames());
        List<string> a = [GkgTunnelComPort, ScannerComPort];
        var ab = a.Except(_comPorts).ToList();
        _comPorts.AddRange(ab);
        this.Load();
    }
}