namespace Projekt_Backend.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }
    public int ClientId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public AppointmentStatus Status { get; set; }

    public string Comment { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;
}
