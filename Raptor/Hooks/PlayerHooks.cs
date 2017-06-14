using System;
using JetBrains.Annotations;
using Raptor.Hooks.Events.Player;
using Terraria;
using Terraria.DataStructures;

namespace Raptor.Hooks
{
    /// <summary>
    ///     Holds the player hooks.
    /// </summary>
    [PublicAPI]
    public static class PlayerHooks
    {
        /// <summary>
        ///     Invoked when a player is being killed.
        /// </summary>
        public static event EventHandler<KillEventArgs> Kill;

        /// <summary>
        ///     Invoked when a player is killed.
        /// </summary>
        public static event EventHandler<KilledEventArgs> Killed;

        /// <summary>
        ///     Invoked when a player is hurt.
        /// </summary>
        public static event EventHandler<HurtEventArgs> Hurt;

        /// <summary>
        ///     Invoked when a player is being hurt.
        /// </summary>
        public static event EventHandler<HurtingEventArgs> Hurting;

        /// <summary>
        ///     Invoked when a player is being updated.
        /// </summary>
        public static event EventHandler<UpdateEventArgs> Update;

        /// <summary>
        ///     Invoked when a player is being updated, before variables are set. Note that this event cannot be handled.
        /// </summary>
        public static event EventHandler<UpdatedEventArgs> Update2;

        /// <summary>
        ///     Invoked when a player is updated.
        /// </summary>
        public static event EventHandler<UpdatedEventArgs> Updated;

        /// <summary>
        ///     Invoked when a player is updated, after variables are set.
        /// </summary>
        public static event EventHandler<UpdatedEventArgs> Updated2;

        internal static void InvokeHurt(object player, object damageSource, int damage, bool isPvP, bool isCritical)
        {
            var args = new HurtEventArgs((Player)player, (PlayerDeathReason)damageSource, damage, isPvP,
                isCritical);
            Hurt?.Invoke(null, args);
        }

        internal static bool InvokeHurting(object player, object damageSource, int damage, bool isPvP, bool isCritical)
        {
            var args = new HurtingEventArgs((Player)player, (PlayerDeathReason)damageSource, damage, isPvP,
                isCritical);
            Hurting?.Invoke(null, args);
            return args.Handled;
        }

        internal static bool InvokeKill(object player, object damageSource, double damage, bool isPvP)
        {
            var args = new KillEventArgs((Player)player, (PlayerDeathReason)damageSource, (int)damage, isPvP);
            Kill?.Invoke(null, args);
            return args.Handled;
        }

        internal static void InvokeKilled(object player, object damageSource, double damage, bool isPvP)
        {
            var args = new KilledEventArgs((Player)player, (PlayerDeathReason)damageSource, (int)damage,
                isPvP);
            Killed?.Invoke(null, args);
        }

        internal static bool InvokeUpdate(object player)
        {
            var args = new UpdateEventArgs((Player)player);
            Update?.Invoke(null, args);
            return args.Handled;
        }

        internal static void InvokeUpdate2(object player)
        {
            var args = new UpdatedEventArgs((Player)player);
            Update2?.Invoke(null, args);
        }

        internal static void InvokeUpdated(object player)
        {
            var args = new UpdatedEventArgs((Player)player);
            Updated?.Invoke(null, args);
        }

        internal static void InvokeUpdated2(object player)
        {
            var args = new UpdatedEventArgs((Player)player);
            Updated2?.Invoke(null, args);
        }
    }
}
