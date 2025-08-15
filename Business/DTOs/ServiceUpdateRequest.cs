using Microsoft.AspNetCore.Http;
using Repository.Models.Enums;

namespace BarbersClub.Business.DTOs;

public record ServiceUpdateRequest
{
        public string? Description { get; set; }
        
        public ServiceStatus? Status { get; set; }
        
        public ServiceTypes? ServiceType { get; set; }

        public IFormFile? UploadedImage { get; set; }
}