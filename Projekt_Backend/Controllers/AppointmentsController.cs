using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt_Backend.DTOs.AppointmentsDTOs;
using Projekt_Backend.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Projekt_Backend.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _service;

    public AppointmentsController(IAppointmentService service)
    {
        _service = service;
    }
    // Ez a művelet lekéri az összes időpontot. Csak adminisztrátorok férhetnek hozzá, ezért az [Authorize(Roles = "Admin")] attribútumot használjuk. Az async/await használata lehetővé teszi, hogy a művelet aszinkron módon fusson, így nem blokkolja a szerver erőforrásait, amíg az adatbázisból lekéri az időpontokat.
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }
    // Ez a művelet lekéri a bejelentkezett felhasználó időpontjait. A JWT tokenből kinyeri a sub claim értékét, ami a clientId-t tartalmazza, majd ezt használja a szolgáltatás GetMyAsync metódusának meghívásához. Ha a token érvénytelen vagy nincs sub claim, akkor Unauthorized választ ad vissza.
    [Authorize]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var clientId))
            return Unauthorized(new { message = "Érvénytelen token." });

        return Ok(await _service.GetMyAsync(clientId));
    }
    // Ez a művelet lemondja a bejelentkezett felhasználó adott időpontját. A JWT tokenből kinyeri a sub claim értékét, ami a clientId-t tartalmazza, majd ezt használja a szolgáltatás CancelMineAsync metódusának meghívásához az időpont azonosítóval. Ha a token érvénytelen vagy nincs sub claim, akkor Unauthorized választ ad vissza. Ha a lemondás nem sikerül (például mert az időpont már megerősített vagy nem létezik), akkor BadRequest választ adunk vissza egy hibaüzenettel. Ha a lemondás sikeres, akkor NoContent választ adunk vissza.
    [Authorize]
    [HttpPost("{id}/cancel-mine")]
    public async Task<IActionResult> CancelMine(int id)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var clientId))
            return Unauthorized();

        var ok = await _service.CancelMineAsync(id, clientId);

        if (!ok)
            return BadRequest(new { message = "Nem lemondható." });

        return NoContent();
    }
    // Ez a művelet létrehoz egy új időpontot a megadott adatok alapján. A JWT tokenből kinyeri a sub claim értékét, ami a clientId-t tartalmazza, majd ezt használja a szolgáltatás CreateAsync metódusának meghívásához az időpont létrehozásához. Ha a token érvénytelen vagy nincs sub claim, akkor Unauthorized választ ad vissza. Ha az időtartam érvénytelen (például mert az end time nem nagyobb, mint a start time), akkor BadRequest választ adunk vissza egy hibaüzenettel. Ha az időpont létrehozása nem sikerül (például mert az időpont már foglalt), akkor Conflict választ adunk vissza egy hibaüzenettel. Ha az időpont sikeresen létrehozva, akkor CreatedAtAction választ adunk vissza, amely tartalmazza az újonnan létrehozott időpont helyét (GetMine művelet) és az új időpont adatait.
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(AppointmentCreateDTO dto)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var clientId))
            return Unauthorized(new { message = "Érvénytelen token." });

        if (dto.EndTime <= dto.StartTime)
            return BadRequest(new { message = "Érvénytelen időtartam." });

        var created = await _service.CreateAsync(clientId, dto);

        if (created == null)
            return Conflict(new { message = "Az időpont már foglalt." }); // 409

        return CreatedAtAction(nameof(GetMine), new { }, created);
    }
    // Ez a művelet megerősíti egy adott időpontot azonosító alapján. Csak adminisztrátorok férhetnek hozzá, ezért az 
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        var ok = await _service.ConfirmAsync(id);

        if (!ok)
            return BadRequest(new { message = "Érvénytelen foglalás vagy ütköző időpont." });

        return NoContent();
    }
    // Ez a művelet lemondja egy adott időpontot azonosító alapján. Csak adminisztrátorok férhetnek hozzá, ezért az [Authorize(Roles = "Admin")] attribútumot használjuk. A szolgáltatás CancelAsync metódusának meghívásával próbáljuk meg lemondani az időpontot. Ha a lemondás nem sikerül (például mert az időpont már megerősített vagy nem létezik), akkor BadRequest választ adunk vissza egy hibaüzenettel. Ha a lemondás sikeres, akkor NoContent választ adunk vissza.
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var ok = await _service.CancelAsync(id);

        if (!ok)
            return BadRequest(new { message = "Érvénytelen foglalás." });

        return NoContent();
    }
    // Ez a művelet lekéri a foglalt időpontokat egy adott intervallumban. Ez hasznos lehet a frontend számára, hogy megjelenítse a szabad és foglalt időpontokat egy naptárban. Az [AllowAnonymous] attribútum lehetővé teszi, hogy bárki hozzáférjen ehhez a művelethez, még akkor is, ha nincs érvényes JWT tokenje. A szolgáltatás GetBusyAsync metódusának meghívásával lekérjük a foglalt időpontokat az adott intervallumban, majd visszaadjuk azokat az Ok válaszban.
    [AllowAnonymous] 
    [HttpGet("busy")]
    public async Task<IActionResult> GetBusy([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        if (to <= from) return BadRequest(new { message = "Érvénytelen intervallum." });

        var busy = await _service.GetBusyAsync(from, to);
        return Ok(busy);
    }
}
