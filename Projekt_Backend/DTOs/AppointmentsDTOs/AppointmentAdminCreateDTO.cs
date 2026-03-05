using System.ComponentModel.DataAnnotations;

namespace Projekt_Backend.DTOs.AppointmentsDTOs;

public class AppointmentAdminCreateDTO
{
    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    // Ha null, akkor "admin blokk" lesz (service-ben admin saját clientId-jára rakjuk)
    public int? ClientId { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }
}
