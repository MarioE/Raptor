using JetBrains.Annotations;

namespace RaptorShock
{
    /// <summary>
    ///     Represents a configuration.
    /// </summary>
    [PublicAPI]
    public class Config
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to show the splash screen.
        /// </summary>
        public bool ShowSplashScreen { get; set; } = true;
    }
}
