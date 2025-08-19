using System.Security.Claims;
using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;

namespace Web.Controllers.WebControllers;

[Authorize]
public class UserProfileController(IUserService userService) : Controller
{

    [HttpGet("profile")]
    public async Task<IActionResult> UserProfile()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized("Sessão de usuário inválida.");
        }
        
        var user = await userService.GetUserByIdAsync(userId.Value);
        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
        }
        
        var userProfileDto = MapUserToDto(user); 
        return View("~/Views/NavBar/UserProfile.cshtml", userProfileDto);
    }
    
    [HttpGet("profile/edit")]
    public async Task<IActionResult> EditProfile()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized("Sessão de usuário inválida.");
        }
        
        var user = await userService.GetUserByIdAsync(userId.Value);
        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
        }
        
        var requestDto = new UserUpdateRequest
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            CurrentProfilePicUrl = user.ProfilePicUrl
        };
        
        return View("~/Views/Navbar/EditProfile.cshtml", requestDto);
    }
    
    [HttpPost("profile/edit")]
    public async Task<IActionResult> EditProfile([FromForm] UserUpdateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Navbar/EditProfile.cshtml", request);
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized("Sessão de usuário inválida.");
        }

        var updatedUser = await userService.UpdateUserAsync(userId.Value, request);

        if (updatedUser == null)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível atualizar o perfil. O e-mail pode já estar em uso.");
            return View("~/Views/Navbar/EditProfile.cshtml", request);
        }

        TempData["SuccessMessage"] = "Perfil atualizado com sucesso!"; 
        return RedirectToAction("UserProfile");
    }

    private UserProfileResponse MapUserToDto(User user)
    {
        return new UserProfileResponse
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePicUrl = user.ProfilePicUrl
        };
    }

    private int? GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdString, out var userId))
        {
            return userId;
        }
        return null;
    }
}