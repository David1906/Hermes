using System;
using Hermes.Cipher.Extensions;
using Hermes.Types;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Hermes.Models;

public class Stop
{
    public static readonly Stop Null = new();

    [Key] public int Id { get; init; }
    public StopType Type { get; set; } = StopType.None;
    public bool IsRestored { get; set; }
    public string Details { get; set; } = "";
    public string Actions { get; set; } = "";
    public DateTime ClosedAt { get; set; }
    public List<Defect> Defects { get; set; } = [];
    public List<User> Users { get; set; } = [];
    [NotMapped] public bool IsNull => this == Null;
    [NotMapped] public bool IsMachineStop => this.Type == StopType.Machine;
    [NotMapped] public string SerialNumber { get; set; } = "";
    [NotMapped] public bool IsFake { get; init; }

    private string? _message;

    [NotMapped]
    public string? Message
    {
        get => _message ?? $"Stop {this.Type.ToTranslatedString()}";
        set => this._message = value;
    }

    [NotMapped] public object UsersConcatenated => string.Join(", ", this.Users.Select(x => x.Name));

    public Stop()
    {
    }

    public Stop(StopType stopType)
    {
        this.Type = stopType;
    }
}