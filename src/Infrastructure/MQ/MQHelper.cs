using IBM.WMQ;
using System.Collections;

namespace Infrastructure.MQ;

public static class MQHelper
{
    public static void AddOptionalProperties(Hashtable connectionProperties, IDictionary<string, object> mqProperties)
    {
        if (mqProperties.ContainsKey(MQC.SSL_CERT_STORE_PROPERTY) && !string.IsNullOrEmpty(mqProperties[MQC.SSL_CERT_STORE_PROPERTY]?.ToString()))
            connectionProperties.Add(MQC.SSL_CERT_STORE_PROPERTY, mqProperties[MQC.SSL_CERT_STORE_PROPERTY]);

        if (mqProperties.ContainsKey(MQC.SSL_CIPHER_SPEC_PROPERTY) && !string.IsNullOrEmpty(mqProperties[MQC.SSL_CIPHER_SPEC_PROPERTY]?.ToString()))
            connectionProperties.Add(MQC.SSL_CIPHER_SPEC_PROPERTY, mqProperties[MQC.SSL_CIPHER_SPEC_PROPERTY]);

        if (mqProperties.ContainsKey(MQC.SSL_PEER_NAME_PROPERTY) && !string.IsNullOrEmpty(mqProperties[MQC.SSL_PEER_NAME_PROPERTY]?.ToString()))
            connectionProperties.Add(MQC.SSL_PEER_NAME_PROPERTY, mqProperties[MQC.SSL_PEER_NAME_PROPERTY]);

        if (mqProperties.ContainsKey(MQC.SSL_RESET_COUNT_PROPERTY) && Convert.ToInt32(mqProperties[MQC.SSL_RESET_COUNT_PROPERTY]) != 0)
            connectionProperties.Add(MQC.SSL_RESET_COUNT_PROPERTY, mqProperties[MQC.SSL_RESET_COUNT_PROPERTY]);

        if (mqProperties.ContainsKey("sslCertRevocationCheck") && Convert.ToBoolean(mqProperties["sslCertRevocationCheck"]))
            MQEnvironment.SSLCertRevocationCheck = true;
    }
}