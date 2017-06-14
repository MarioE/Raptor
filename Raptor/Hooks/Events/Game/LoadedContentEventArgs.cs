using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Content;

namespace Raptor.Hooks.Events.Game
{
    /// <summary>
    ///     Provides data for the <see cref="GameHooks.LoadedContent" /> event.
    /// </summary>
    [PublicAPI]
    public sealed class LoadedContentEventArgs : EventArgs
    {
        internal LoadedContentEventArgs(ContentManager contentManager)
        {
            ContentManager = contentManager;
        }

        /// <summary>
        ///     Gets the content manager.
        /// </summary>
        [NotNull]
        public ContentManager ContentManager { get; }
    }
}
