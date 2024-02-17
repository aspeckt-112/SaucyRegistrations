namespace Saucy.Common.Enums
{
    /// <summary>
    /// The ServiceScope enum is used to define the scope of a service.
    /// </summary>
    public enum ServiceScope
    {
        /// <summary>
        /// A transient service is created each time it is requested.
        /// </summary>
        Transient,

        /// <summary>
        /// A singleton service is created once and shared across all requests.
        /// </summary>
        Singleton,

        /// <summary>
        /// A scoped service is created once per request.
        /// </summary>
#pragma warning disable SA1413
        Scoped
#pragma warning restore SA1413
    }
}