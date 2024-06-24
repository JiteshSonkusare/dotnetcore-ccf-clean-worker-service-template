using PureCloudPlatform.Client.V2.Client;

namespace Application.Common.Interfaces.Genesys;

public interface IGenesysAuthHandler
{
    Task AuthenticateAsync(Configuration configuration);
}