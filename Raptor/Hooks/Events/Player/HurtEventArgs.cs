using System;
using JetBrains.Annotations;
using Terraria;
using Terraria.DataStructures;

namespace Raptor.Hooks.Events.Player
{
    /// <summary>
    ///     Provides the data for the <see cref="PlayerHooks.Hurt" /> event.
    /// </summary>
    [PublicAPI]
    public class HurtEventArgs : EventArgs
    {
        internal HurtEventArgs(Terraria.Player player, PlayerDeathReason damageSource, int damage, bool isPvP,
            bool isCritical)
        {
            Player = player;
            DamageSource = damageSource;
            Damage = damage;
            IsPvP = isPvP;
            IsCritical = isCritical;
        }

        /// <summary>
        ///     Gets the damage.
        /// </summary>
        public int Damage { get; }

        /// <summary>
        ///     Gets the damage source.
        /// </summary>
        [NotNull]
        public PlayerDeathReason DamageSource { get; }

        /// <summary>
        ///     Gets a value indicating whether the damage is critical.
        /// </summary>
        public bool IsCritical { get; }

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
        [NotNull]
        public Terraria.Player Player { get; }
    }
}
