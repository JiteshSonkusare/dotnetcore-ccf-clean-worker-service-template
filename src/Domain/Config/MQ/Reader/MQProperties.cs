using System.ComponentModel.DataAnnotations;

namespace Domain.Config.MQ.Reader;


public record MQProperties
{
    [Required(ErrorMessage = "HostName required!")]
    public string HostName { get; set; } = null!;

    [Required(ErrorMessage = "Port required!")]
    public string? Port { get; set; } = null!;

    [Required(ErrorMessage = "Channel required!")]
    public string? Channel { get; set; } = null!;

    public string? SSLCertStore { get; set; }

    public string? SSLCipherSpec { get; set; }

    public string? SSLPeerName { get; set; }

    public string? SSLResetCount { get; set; }

    public bool SSLCertRevocationCheck { get; set; }
}
