using System;

namespace Bespoke.Common.Net
{
#if !WINDOWS    
    /// <summary>
    /// Defines transport types for network communication.
    /// </summary>
    public enum TransportType
    {
        /// <summary>
        /// Udp transport
        /// </summary>
        Udp = 1,

        /// <summary>
        /// Tcp transport.
        /// </summary>
        Tcp = 2,
    }
#endif
}
