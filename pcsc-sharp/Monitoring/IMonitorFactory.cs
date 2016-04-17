using System;
using System.Collections.Generic;

namespace PCSC
{
    /// <summary>
    /// Smart card monitor factory
    /// </summary>
    public interface IMonitorFactory
    {
        /// <summary>
        ///  Creates a smart card event monitor 
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <returns>A <see cref="ISCardMonitor"/></returns>
        ISCardMonitor Create(SCardScope scope);

        /// <summary>
        /// Creates and starts a smart card event monitor for a specific reader
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <param name="readerName">Name of the smart card reader that shall be monitored.</param>
        /// <returns>A started <see cref="ISCardMonitor"/></returns>
        ISCardMonitor Start(SCardScope scope, string readerName);

        /// <summary>
        /// Creates and starts a smart card event monitor for one or more readers.
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <param name="readerNames">Names of the smart card readers that shall be monitored.</param>
        /// <returns>A started <see cref="ISCardMonitor"/></returns>
        ISCardMonitor Start(SCardScope scope, IEnumerable<string> readerNames);

        /// <summary>
        /// Creates and starts a smart card event monitor for one or more readers.
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <param name="readerNames">Names of the smart card readers that shall be monitored.</param>
        /// <param name="preStartAction">Action that will be invoked prior monitor start</param>
        /// <returns>A started <see cref="ISCardMonitor"/></returns>
        ISCardMonitor Start(SCardScope scope, IEnumerable<string> readerNames, Action<ISCardMonitor> preStartAction);

        /// <summary>
        /// Releases the smart card monitor and its dependencies using the <see cref="IDisposable.Dispose"/> method.
        /// </summary>
        /// <param name="monitor">Smart card monitor that shall be stopped and disposed.</param>
        void Release(ISCardMonitor monitor);
    }
}