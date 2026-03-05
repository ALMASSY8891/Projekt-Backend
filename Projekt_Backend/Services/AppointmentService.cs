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
    // Ez a művelet lekéri az összes időpontot. Csak adminisztrátorok férhetnek hozzá, ezért az [Authorize
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
    // Ez a művelet lekéri a bejelentkezett felhasználó időpontjait. A JWT tokenből kinyeri a sub claim értékét, ami a clientId-t tartalmazza, majd ezt használja a szolgáltatás GetMyAsync metódusának meghívásához. Ha a token érvénytelen vagy nincs sub claim, akkor Unauthorized választ ad vissza.
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
    // Ez a művelet létrehoz egy új időpontot a megadott adatok alapján. A JWT tokenből kinyeri a sub claim értékét, ami a clientId-t tartalmazza, majd ezt használja a szolgáltatás CreateAsync metódusának meghívásához az időpont létrehozásához. Ha a token érvénytelen vagy nincs sub claim, akkor Unauthorized választ ad vissza. Ha az időtartam érvénytelen (például mert az end time nem nagyobb, mint a start time), akkor BadRequest választ adunk vissza egy hibaüzenettel. Ha az időpont létrehozása nem sikerül (például mert az időpont már foglalt), akkor Conflict választ adunk vissza egy hibaüzenettel. Ha az időpont sikeresen létrehozva, akkor CreatedAtAction választ adunk vissza, amely tartalmazza az újonnan létrehozott időpont helyét (GetMine művelet) és az új időpont adatait.
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
    public async Task<AppointmentResponseDTO?> CreateAdminAsync(int adminClientId, AppointmentAdminCreateDTO dto)
    {
        // adminClientId létezés ellenőrzés (jó védelem)
        var adminExists = await _db.Clients.AnyAsync(c => c.ClientId == adminClientId);
        if (!adminExists) return null; // vagy dobj hibát

        var conflict = await _db.Appointments.AnyAsync(a =>
            (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Pending) &&
            a.StartTime < dto.EndTime &&
            a.EndTime > dto.StartTime
        );

        if (conflict) return null;

        var entity = new Appointment
        {
            ClientId = adminClientId,               
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = AppointmentStatus.Confirmed,    // admin blokk: én Confirmed-et javaslok
            Comment = dto.Comment?.Trim() ?? "Admin által rögzítve"
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
            Comment = entity.Comment,
            ClientEmail = null // vagy lekérdezheted, ha kell
        };
    }
    // Ez a művelet lemondja a bejelentkezett felhasználó adott időpontját. A JWT tokenből kinyeri a sub claim értékét, ami a clientId-t tartalmazza, majd ezt használja a szolgáltatás CancelMineAsync metódusának meghívásához az időpont azonosítóval. Ha a token érvénytelen vagy nincs sub claim, akkor Unauthorized választ ad vissza. Ha a lemondás nem sikerül (például mert az időpont már megerősített vagy nem létezik), akkor BadRequest választ adunk vissza egy hibaüzenettel. Ha a lemondás sikeres, akkor NoContent választ adunk vissza.
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
    // Ez a művelet lemondja a bejelentkezett felhasználó adott időpontját. A JWT tokenből kinyeri a sub claim értékét, ami a clientId-t tartalmazza, majd ezt használja a szolgáltatás CancelMineAsync metódusának meghívásához az időpont azonosítóval. Ha a token érvénytelen vagy nincs sub claim, akkor Unauthorized választ ad vissza. Ha a lemondás nem sikerül (például mert az időpont már megerősített vagy nem létezik), akkor BadRequest választ adunk vissza egy hibaüzenettel. Ha a lemondás sikeres, akkor NoContent választ adunk vissza.
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
    // Ez a művelet lemondja a bejelentkezett felhasználó adott időpontját. A JWT tokenből kinyeri a sub claim értékét, ami a clientId-t tartalmazza, majd ezt használja a szolgáltatás CancelMineAsync metódusának meghívásához az időpont azonosítóval. Ha a token érvénytelen vagy nincs sub claim, akkor Unauthorized választ ad vissza. Ha a lemondás nem sikerül (például mert az időpont már megerősített vagy nem létezik), akkor BadRequest választ adunk vissza egy hibaüzenettel. Ha a lemondás sikeres, akkor NoContent választ adunk vissza.
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
    // Ez a művelet lekéri az adott időintervallumban foglalt időpontokat. Csak a megerősített és függőben lévő időpontokat vesszük figyelembe, mert a törölt vagy teljesített időpontok már nem foglalnak helyet. Az AsNoTracking használata optimalizálja a lekérdezést, mivel csak olvasunk, nem módosítunk entitásokat.
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
