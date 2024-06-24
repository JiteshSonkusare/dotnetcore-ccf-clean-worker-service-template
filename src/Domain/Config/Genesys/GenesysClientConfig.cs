using System.ComponentModel.DataAnnotations;

namespace Domain.Config.Genesys;

public record GenesysClientConfig
{
    [Required(ErrorMessage = "ClientId required!")]
    public string? ClientId { get; set; }

    [Required(ErrorMessage = "Client Secret required!")]
    public string? ClientSecret { get; set; }

    [Required(ErrorMessage = "Region required!")]
    public string? Region { get; set; }
}