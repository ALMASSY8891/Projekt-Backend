namespace Projekt_Backend.Models;
// Az időpontfoglalás státuszát jelző enum, amely a foglalás aktuális állapotát jelzi, és amelyet a rendszer vagy a felhasználó frissíthet a foglalás feldolgozása során.
public enum AppointmentStatus
{
    Pending = 0,
    Confirmed = 1,
    Completed = 2,
    Cancelled = 3
}