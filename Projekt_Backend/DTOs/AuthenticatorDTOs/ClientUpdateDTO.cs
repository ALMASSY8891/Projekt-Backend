using System.ComponentModel.DataAnnotations;

namespace Projekt_Backend.Dtos.Client
{
    public sealed class ClientUpdateDto
    {
      
        public string Name { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
    }
}