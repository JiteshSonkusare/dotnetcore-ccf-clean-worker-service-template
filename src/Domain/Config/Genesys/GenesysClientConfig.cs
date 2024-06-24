using System.ComponentModel.DataAnnotations;

namespace Domain.Config.Genesys;

public record GenesysClientConfig
{
    [Required(ErrorMessage = "ClientId required!")]
    public string ClientId { get; set; } = null!;

    [Required(ErrorMessage = "Client Secret required!")]
    public string ClientSecret { get; set; } = null!;

    [Required(ErrorMessage = "Region required!")]
    public string Region { get; set; } = null!;
}