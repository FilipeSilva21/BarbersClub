using Microsoft.AspNetCore.Authorization; // Adicionado
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;

namespace BarbersClub.Controllers.ServiceControllers;

[Route("api/services")]
[ApiController]
public class ServiceApiController(IServiceService serviceService, IBarberShopService barberShopService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateService([FromBody] ServiceRegisterRequest request)
    {
        var createdService = await serviceService.CreateServiceAsync(request);
        
        return CreatedAtAction(nameof(GetServicesByBarberShop), new { barberShopId = createdService.BarberShopId }, createdService);
    }

    [HttpGet("myServices")]
    public async Task<IActionResult> GetMyServices([FromQuery] string status)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            return Unauthorized("Sessão inválida.");
        
        var services = await serviceService.GetServicesByUserAndStatusAsync(userId, status); 
        
        return Ok(services);
    }

    [HttpPost("cancel/{serviceId}")]
    [Authorize]
    public async Task<IActionResult> CancelService(int serviceId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }
        
        var serviceToCheck = await serviceService.GetServiceByIdAsync(serviceId);
        if (serviceToCheck is null)
        {
            return Unauthorized();
        }

        var result = await serviceService.CancelServiceAsync(serviceId, userId);
    
        if (!result)
        {
            return BadRequest("Não foi possível cancelar o agendamento.");
        }

        return Ok(new { message = "Agendamento cancelado com sucesso." });
    }
    
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> GetServicesByUser(int userId)
    {
        var services = await serviceService.GetServicesByUserAsync(userId);
        return Ok(services);
    }

    [HttpGet]
    public async Task<IActionResult> GetServices(
        [FromQuery] string? barberShopName,
        [FromQuery] string? clientName,
        [FromQuery] string? serviceType,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] bool? hasPhoto
        ,[FromQuery] string? barber,
        [FromQuery] string? sortBy)
    {
        var services = await serviceService.GetServicesAsync(
            barberShopName, 
            serviceType,
            barber,
            startDate, 
            endDate,
            hasPhoto, 
            sortBy
        );
        
        return Ok(services);
    }

    [HttpGet("barbershop/{barberShopId}")]
    public async Task<IActionResult> GetServicesByBarberShop(
        int barberShopId, 
        [FromQuery] string? clientName, 
        [FromQuery] string? serviceType, 
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate,
        [FromQuery] TimeSpan? time)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized("Sessão inválida.");
        }

        var barberShop = await barberShopService.GetBarberShopByIdAsync(barberShopId);
        if (barberShop == null || barberShop.UserId != userId)
        {
            return Forbid(); 
        }

        var services = await serviceService.GetServicesByBarberShopAsync(
            barberShopId, 
            clientName, 
            serviceType, 
            startDate, 
            endDate,
            time);

        return Ok(services);
    }
    
    [HttpGet("bookedTimes")]
    public async Task<IActionResult> GetBookedTimes([FromQuery] int barberShopId, [FromQuery] DateTime date)
    {
        if (barberShopId <= 0)
        {
            return BadRequest("O ID da barbearia é inválido.");
        }
        try
        {
            var barberShop = await barberShopService.GetBarberShopByIdAsync(barberShopId);

            if (barberShop == null)
                return NotFound(new { message = "Barbearia não encontrada." });
            
            if (string.IsNullOrEmpty(barberShop.OpeningHours) || string.IsNullOrEmpty(barberShop.ClosingHours))
                return BadRequest(new { message = "Os horários de funcionamento para esta barbearia não estão definidos." });

            var bookedTimesList = await serviceService.GetBookedTimesAsync(barberShopId, date);

            var response = new
            {
                openingTime = barberShop.OpeningHours,
                closingTime = barberShop.ClosingHours,
                bookedTimes = bookedTimesList
            };
        
            return Ok(response);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Ocorreu um erro interno ao buscar os horários.");
        }
    }
    
    [HttpPost("conclude/{serviceId}")]
    public async Task<IActionResult> ConcludeService(int serviceId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
            return Unauthorized("Sessão inválida.");

        var service = await serviceService.GetServiceByIdAsync(serviceId);
        if (service == null)
            return NotFound("Agendamento não encontrado.");

        if (service.BarberShop.UserId != userId)
            return Forbid();

        var result = await serviceService.ConcludeServiceAsync(serviceId); 
        if (!result)
            return BadRequest(new { message = "Não foi possível concluir o agendamento." });

        return Ok(new { message = "Agendamento concluído com sucesso." });
    }

    [HttpPost("conclude-with-photo/{serviceId}")]
    public async Task<IActionResult> ConcludeWithPhoto(int serviceId, [FromForm] IFormFile photoFile)
    {
        if (photoFile == null || photoFile.Length == 0)
            return BadRequest(new { message = "Nenhum arquivo foi enviado." });

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
            return Unauthorized("Sessão inválida.");

        var service = await serviceService.GetServiceByIdAsync(serviceId);
        if (service == null)
            return NotFound("Agendamento não encontrado.");

        if (service.BarberShop.UserId != userId)
            return Forbid();

        try
        {
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "services");
            if (!Directory.Exists(uploadsFolderPath))
                Directory.CreateDirectory(uploadsFolderPath);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photoFile.CopyToAsync(stream);
            }

            var dbPath = $"/images/services/{uniqueFileName}";

            var result = await serviceService.ConcludeServiceAsync(serviceId, dbPath);
            
            if (!result)
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                return BadRequest(new { message = "Não foi possível atualizar o agendamento no banco de dados." });
            }

            return Ok(new { message = "Agendamento concluído com foto.", path = dbPath });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro interno ao processar o arquivo." });
        }
    }
    
    [HttpPut("update/{serviceId}")] 
    [Authorize] 
    public async Task<IActionResult> UpdateService(int serviceId, [FromForm] ServiceUpdateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedService = await serviceService.UpdateServiceAsync(serviceId, request);

        if (updatedService == null)
        {
            return NotFound(new { message = "Serviço não encontrado." });
        }

        return Ok(updatedService);
    }
}
