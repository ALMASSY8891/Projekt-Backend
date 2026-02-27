namespace Projekt_Backend.DTOs.AppointmentsDTOs;

public class AppointmentResponseDTO
{
    public int AppointmentId { get; set; }
    public int ClientId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Status { get; set; } // frontendnek int
    public string? Comment { get; set; }
    public string? ClientEmail { get; set; } // admin listához
}