using System.ComponentModel;
using JetBrains.Annotations;
using Terraria;

namespace Raptor.Hooks.Events.Player
{
    /// <summary>
    ///     Provides data for the <see cref="PlayerHooks.Update" /> and <see cref="PlayerHooks.Update2" /> events.
    /// </summary>
    [PublicAPI]
    public sealed class UpdateEventArgs : HandledEventArgs
    {
        internal UpdateEventArgs(Terraria.Player player)
        {
            Player = player;
        }

        /// <summary>
        ///     Gets a value indicating whether the player is the local player.
        /// </summary>
        public bool IsLocal => Player.whoAmI == Main.myPlayer;

        /// <summary>
        ///     Gets the player.
        /// </summary>
        [NotNull]
        public Terraria.Player Player { get; }
    }
}
