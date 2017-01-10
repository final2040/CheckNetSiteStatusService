using System;

namespace Data.NetworkTest
{
    /// <summary>
    /// Representa el estado de la conexi�n.
    /// </summary>
    [Obsolete("Utilize los objetos State en su lugar")]
    public enum ConnectionStatus
    {
        ConnectionOnline,
        ConnectionOffline,
        Retrying
    }
}