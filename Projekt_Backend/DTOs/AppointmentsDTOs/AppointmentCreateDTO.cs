namespace Projekt_Backend.DTOs.AppointmentsDTOs;

public class AppointmentCreateDTO
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Comment { get; set; }
}