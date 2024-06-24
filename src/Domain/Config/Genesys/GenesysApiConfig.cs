using System.ComponentModel.DataAnnotations;

namespace Domain.Config.Genesys;

public record GenesysApiConfig
{
    [Required(ErrorMessage = "Datatable name required!")]
    public string DatatableName { get; set; } = null!;
}
