using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Terraria;
using Terraria.DataStructures;

namespace Raptor.Hooks.Events.Player
{
    /// <summary>
    ///     Provides data for the <see cref="PlayerHooks.Kill" /> event.
    /// </summary>
    [PublicAPI]
    public sealed class KillEventArgs : HandledEventArgs
    {
        internal KillEventArgs(Terraria.Player player, PlayerDeathReason damageSource, int damage, bool isPvP)
        {
            Player = player;
            DamageSource = damageSource;
            Damage = damage;
            IsPvP = isPvP;
        }

        /// <summary>
        ///     Gets the damage.
        /// </summary>
        public int Damage { get; }

        /// <summary>
        ///     Gets the damage source.
        /// </summary>
        [CLSCompliant(false)]
        [NotNull]
        public PlayerDeathReason DamageSource { get; }

        /// <summary>
        ///     Gets a value indicating whether the player is the local player.
        /// </summary>
        public bool IsLocal => Player.whoAmI == Main.myPlayer;

        /// <summary>
        ///     Gets a value indicating whether the damage is from PvP.
        /// </summary>
        public bool IsPvP { get; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        [CLSCompliant(false)]
        [NotNull]
        public Terraria.Player Player { get; }
    }
}
