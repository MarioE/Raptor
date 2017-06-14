using System;
using JetBrains.Annotations;

namespace Raptor.Api
{
    /// <summary>
    ///     Specifies a plugin.
    /// </summary>
    [PublicAPI]
    public abstract class TerrariaPlugin : IDisposable
    {
        /// <summary>
        ///     Gets the author.
        /// </summary>
        [NotNull]
        public virtual string Author => "";

        /// <summary>
        ///     Gets the description.
        /// </summary>
        [NotNull]
        public virtual string Description => "";

        /// <summary>
        ///     Gets the name.
        /// </summary>
        [NotNull]
        public virtual string Name => "";
        
        /// <summary>
        ///     Gets the version.
        /// </summary>
        [NotNull]
        public virtual Version Version => new Version(1, 0);

        /// <summary>
        ///     Disposes the plugin.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disposes the plugin.
        /// </summary>
        /// <param name="disposing"><c>true</c> to dispose managed resources; otherwise, <c>false</c>.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        ~TerrariaPlugin()
        {
            Dispose(false);
        }
    }
}
