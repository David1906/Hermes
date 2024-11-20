using System;
using System.ComponentModel.DataAnnotations;

namespace Hermes.Models;

public class DboUpdate
{
    [Key] public string Name { get; set; } = "";
    public DateTime LastModified { get; set; }
    public string Description { get; set; } = "";
}