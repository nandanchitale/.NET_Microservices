using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models;

public class Command
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string HowTo { get; set; }

    [Required]
    public int CommandLine { get; set; }

    [Required]
    public int PlatformId { get; set; } // Foreign key to Platform

    public Platform Platform { get; set; } // Navigation Property
}