﻿using Hermes.Cipher.Extensions;
using Hermes.Types;
using System.IO.Ports;

namespace Hermes.Models;

public class Settings
{
    #region General

    public LanguageType Language { get; set; } = LanguageType.En;
    public int WaitDelayMilliseconds { get; set; } = 100;
    public string DatabaseServer { get; set; } = "10.12.204.254";
    public StationType Station { get; set; } = StationType.SpiBottom;
    public MachineType Machine { get; set; } = MachineType.Spi;
    public string StationId { get; set; } = "";
    public LineType Line { get; set; } = LineType.Ag01;
    public LogfileType LogfileType { get; set; } = LogfileType.TriDefault;
    public FileExtension SfcResponseExtension { get; set; } = FileExtension.Ret;
    public FileExtension InputFileExtension { get; set; } = FileExtension._3dx;
    public bool SendRepairFile { get; set; } = true;

    #endregion

    #region UutProcessor

    public bool AutostartUutProcessor { get; set; } = true;
    public string InputPath { get; set; } = @".\Input";
    public string BackupPath { get; set; } = @".\Backup";
    public string SfcPath { get; set; } = @"Z:\";
    public int SfcTimeoutSeconds { get; set; } = 10;
    public int UutSuccessWindowTimeoutSeconds { get; set; } = 5;
    public int MaxSfcRetries { get; set; } = 1;
    public string GkgTunnelComPort { get; set; } = "COM50";
    public string ScannerComPort { get; set; } = "COM40";
    public int ScannerComBaudRate { get; set; } = 115200;
    public int ScannerComDataBits { get; set; } = 8;
    public Parity ScannerComParity { get; set; } = Parity.None;
    public StopBits ScannerComStopBits { get; set; } = StopBits.One;
    public string AdditionalOkSfcResponse { get; set; } = "";
    public string InputFileFilter => "*" + InputFileExtension.GetDescription();
    public string ResponseFileFilter => "*" + SfcResponseExtension.GetDescription();
    public bool EnableCriticalLocationStop { get; set; } = true;
    public bool EnableRuleThreeFiveTen { get; set; } = true;
    public bool EnableMachineStop { get; set; } = true;
    public string CriticalLocations { get; set; } = "U1";
    public string UpdateManagerUrl { get; set; } = @"http://10.12.204.254/hermes/download";
    public string ScannerFilter { get; set; } = @"1A\w+$";
    public bool AutoSyncDatabase { get; set; } = true;
    public bool DeleteOldBackupFilesOnSunday { get; set; } = true;
    public bool OverwritePackageId { get; set; } = true;

    #endregion

    public string GetFirstCriticalDefectLocation()
    {
        return CriticalLocations.Split(',')[0];
    }
}