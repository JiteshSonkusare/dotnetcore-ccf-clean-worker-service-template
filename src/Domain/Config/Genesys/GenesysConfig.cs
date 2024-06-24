namespace Domain.Config.Genesys;

public class GenesysConfig
{
    public const string SectionName = "GenesysCloud";

    public GenesysClientConfig ClientDetails { get; set; } = null!;

    public GenesysApiConfig ApiDetails { get; set; } = null!;
}