using Projekt_Backend.DTOs.AppointmentsDTOs;
// Az IAppointmentService interfész meghatározza a szolgáltatás által kínált műveleteket, amelyek az időpontok kezelésére szolgálnak. Ez magában foglalja az időpontok lekérdezését, létrehozását, megerősítését és törlését, valamint a foglalt időpontok lekérdezését egy adott időszakban.
public interface IAppointmentService
{
    Task<List<AppointmentResponseDTO>> GetAllAsync();
    Task<List<AppointmentResponseDTO>> GetMyAsync(int clientId);
    Task<AppointmentResponseDTO?> CreateAsync(int clientId, AppointmentCreateDTO dto);
    Task<AppointmentResponseDTO?> CreateAdminAsync(int adminClientId, AppointmentAdminCreateDTO dto);

    Task<bool> ConfirmAsync(int appointmentId);
    Task<bool> CancelAsync(int appointmentId);

    Task<bool> CancelMineAsync(int appointmentId, int clientId);
    Task<List<BusySlotDTO>> GetBusyAsync(DateTime from, DateTime to);
}