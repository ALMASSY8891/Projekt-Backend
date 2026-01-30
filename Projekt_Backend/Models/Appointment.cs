using System;
using System.Collections.Generic;

namespace Projekt_Backend.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int ClientId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int Status { get; set; }

    public string Comment { get; set; } = null!;
}
