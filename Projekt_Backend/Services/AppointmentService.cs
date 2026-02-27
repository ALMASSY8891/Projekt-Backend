using Microsoft.EntityFrameworkCore;
using Projekt_Backend.DTOs.AppointmentsDTOs;
using Projekt_Backend.Models;
using Projekt_Backend.Services.Interfaces;


namespace Projekt_Backend.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AcsolasContext _db;

    public AppointmentService(AcsolasContext db)
    {
        _db = db;
    }

    public async Task<List<AppointmentResponseDTO>> GetAllAsync()
    {
        return await _db.Appointments
            .AsNoTracking()
            .OrderByDescending(a => a.StartTime)
            .Select(a => new AppointmentResponseDTO
            {
                AppointmentId = a.AppointmentId,
                ClientId = a.ClientId,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = (int)a.Status,
                Comment = a.Comment,
                ClientEmail = a.Client.Email
            })
            .ToListAsync();
    }

    public async Task<List<AppointmentResponseDTO>> GetMyAsync(int clientId)
    {
        return await _db.Appointments
            .AsNoTracking()
            .Where(a => a.ClientId == clientId)
            .OrderByDescending(a => a.StartTime)
            .Select(a => new AppointmentResponseDTO
            {
                AppointmentId = a.AppointmentId,
                ClientId = a.ClientId,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = (int)a.Status,
                Comment = a.Comment
            })
            .ToListAsync();
    }

    public async Task<AppointmentResponseDTO?> CreateAsync(int clientId, AppointmentCreateDTO dto)
    {
        var conflict = await _db.Appointments.AnyAsync(a =>
    (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Pending) &&
    a.StartTime < dto.EndTime &&
    a.EndTime > dto.StartTime
);

        if (conflict)
            return null; // controller 409-et ad

        var entity = new Appointment
        {
            ClientId = clientId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = AppointmentStatus.Pending,
            Comment = dto.Comment?.Trim() ?? ""
        };

        _db.Appointments.Add(entity);
        await _db.SaveChangesAsync();

        return new AppointmentResponseDTO
        {
            AppointmentId = entity.AppointmentId,
            ClientId = entity.ClientId,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            Status = (int)entity.Status,
            Comment = entity.Comment
        };
    }

    public async Task<bool> ConfirmAsync(int appointmentId)
    {
        var appt = await _db.Appointments
            .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

        if (appt == null)
            return false;

        if (appt.Status != AppointmentStatus.Pending)
            return false;

        // Ütközésellenőrzés
        var overlaps = await _db.Appointments.AnyAsync(a =>
            a.AppointmentId != appt.AppointmentId &&
            a.Status == AppointmentStatus.Confirmed &&
            a.StartTime < appt.EndTime &&
            a.EndTime > appt.StartTime
        );

        if (overlaps)
            return false;

        appt.Status = AppointmentStatus.Confirmed;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(int appointmentId)
    {
        var appt = await _db.Appointments
            .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

        if (appt == null)
            return false;

        if (appt.Status == AppointmentStatus.Completed)
            return false;

        appt.Status = AppointmentStatus.Cancelled;

        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> CancelMineAsync(int appointmentId, int clientId)
    {
        var appt = await _db.Appointments
            .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

        if (appt == null)
            return false;

        // csak a sajátját törölheti
        if (appt.ClientId != clientId)
            return false;

        // Completed ne legyen lemondható
        if (appt.Status == AppointmentStatus.Completed)
            return false;

        // opcionális: időkorlát (pl. 24 óra)
        if (appt.StartTime <= DateTime.UtcNow.AddHours(24))
            return false;

        appt.Status = AppointmentStatus.Cancelled;

        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<List<BusySlotDTO>> GetBusyAsync(DateTime from, DateTime to)
    {
        return await _db.Appointments
     .AsNoTracking()
     .Where(a =>
         (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Pending) &&
         a.StartTime < to &&
         a.EndTime > from
     )
     .OrderBy(a => a.StartTime)
     .Select(a => new BusySlotDTO
     {
         StartTime = a.StartTime,
         EndTime = a.EndTime
     })
     .ToListAsync();  
    }
    
}
