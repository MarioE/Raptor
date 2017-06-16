using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Terraria;

namespace Raptor.Hooks.Events.Game
{
    /// <summary>
    ///     Provides data for the <see cref="GameHooks.Lighting" /> event.
    /// </summary>
    [PublicAPI]
    public sealed class LightingEventArgs : HandledEventArgs
    {
        internal LightingEventArgs(Lighting.LightingSwipeData swipeData)
        {
            SwipeData = swipeData;
        }

        /// <summary>
        ///     Gets the swipe data.
        /// </summary>
        [CLSCompliant(false)]
        [NotNull]
        public Lighting.LightingSwipeData SwipeData { get; }
    }
}
