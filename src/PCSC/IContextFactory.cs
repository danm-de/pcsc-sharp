namespace PCSC
{
    /// <summary>
    /// Smart card context factory
    /// </summary>
    public interface IContextFactory
    {
        /// <summary>
        /// Create and establish a new smart card context.
        /// </summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <returns>A new established smart card context</returns>
        ISCardContext Establish(SCardScope scope);
    }
}