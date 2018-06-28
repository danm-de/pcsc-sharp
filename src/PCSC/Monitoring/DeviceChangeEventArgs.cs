using System;
using System.Collections.Generic;

namespace PCSC.Monitoring
{
    /// <summary>Information about attached and detached smart card reader devices.</summary>
    public class DeviceChangeEventArgs : EventArgs
    {
        /// <summary>
        /// All connected smartcard reader devices
        /// </summary>
        public IEnumerable<string> AllReaders { get; }

        /// <summary>
        /// Recently attached (added) smartcard reader devices
        /// </summary>
        public IEnumerable<string> AttachedReaders { get; }

        /// <summary>
        /// Recently detached (removed) smartcard reader devices
        /// </summary>
        public IEnumerable<string> DetachedReaders { get; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="allReaders">All connected smartcard reader devices</param>
        /// <param name="attachedReaders">Attached/added smartcard reader devices</param>
        /// <param name="detachedReaders">Detached/removed smartcard reader devices</param>
        public DeviceChangeEventArgs(IEnumerable<string> allReaders, IEnumerable<string> attachedReaders,
            IEnumerable<string> detachedReaders) {
            AllReaders = allReaders;
            AttachedReaders = attachedReaders;
            DetachedReaders = detachedReaders;
        }
    }
}
