using System;
using JetBrains.Annotations;
using Terraria;

namespace Raptor.Api
{
    /// <summary>
    ///     Specifies a plugin.
    /// </summary>
    [PublicAPI]
    public abstract class TerrariaPlugin : IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TerrariaPlugin" /> class with the specified Main instance.
        /// </summary>
        /// <param name="main">The Main instance, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="main" /> is <c>null</c>.</exception>
        [CLSCompliant(false)]
        protected TerrariaPlugin([NotNull] Main main)
        {
            Main = main ?? throw new ArgumentNullException(nameof(main));
        }

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
        ///     Gets the Main instance.
        /// </summary>
        [CLSCompliant(false)]
        public Main Main { get; }

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

        /// <summary>
        ///     Destructs the plugin.
        /// </summary>
        ~TerrariaPlugin()
        {
            Dispose(false);
        }
    }
}
