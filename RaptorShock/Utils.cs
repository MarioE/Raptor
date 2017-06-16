using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Terraria;

namespace RaptorShock
{
    /// <summary>
    ///     Provides utility methods.
    /// </summary>
    [PublicAPI]
    public static class Utils
    {
        private static readonly Dictionary<string, int> ItemNamesToIds = new Dictionary<string, int>();
        private static readonly Dictionary<string, int> ProjectileNamesToIds = new Dictionary<string, int>();

        /// <summary>
        ///     Gets the local player.
        /// </summary>
        [CLSCompliant(false)]
        [NotNull]
        public static Player LocalPlayer => Main.player[Main.myPlayer];

        /// <summary>
        ///     Gets the local player's selected item.
        /// </summary>
        [CLSCompliant(false)]
        [NotNull]
        public static Item LocalPlayerItem => LocalPlayer.inventory[LocalPlayer.selectedItem];

        /// <summary>
        ///     Gets the items matching a name or ID.
        /// </summary>
        /// <param name="nameOrId">The name or ID, which must not be <c>null</c>.</param>
        /// <returns>The list of items.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="nameOrId" /> is <c>null</c>.</exception>
        [CLSCompliant(false)]
        [ItemNotNull]
        [NotNull]
        public static List<Item> GetItemsByNameOrId([NotNull] string nameOrId)
        {
            if (nameOrId == null)
            {
                throw new ArgumentNullException(nameof(nameOrId));
            }

            if (int.TryParse(nameOrId, out var id) && id > 0 && id < Main.maxItemTypes)
            {
                var item = new Item();
                item.SetDefaults(id);
                return new List<Item> {item};
            }

            var items = new List<Item>();
            foreach (var kvp in ItemNamesToIds)
            {
                var name = kvp.Key;
                id = kvp.Value;
                // Check item names with CurrentCultureIgnoreCase.
                if (name.Equals(nameOrId, StringComparison.CurrentCultureIgnoreCase))
                {
                    var item = new Item();
                    item.SetDefaults(id);
                    return new List<Item> {item};
                }
                if (name.StartsWith(nameOrId, StringComparison.CurrentCultureIgnoreCase))
                {
                    var item = new Item();
                    item.SetDefaults(id);
                    items.Add(item);
                }
            }
            return items;
        }

        /// <summary>
        ///     Gets the projectiles matching a name or ID.
        /// </summary>
        /// <param name="nameOrId">The name or ID, which must not be <c>null</c>.</param>
        /// <returns>The list of projectiles.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="nameOrId" /> is <c>null</c>.</exception>
        [CLSCompliant(false)]
        [ItemNotNull]
        [NotNull]
        public static List<Projectile> GetProjectilesByNameOrId([NotNull] string nameOrId)
        {
            if (nameOrId == null)
            {
                throw new ArgumentNullException(nameof(nameOrId));
            }

            if (int.TryParse(nameOrId, out var id) && id > 0 && id < Main.maxProjectileTypes)
            {
                var projectile = new Projectile();
                projectile.SetDefaults(id);
                return new List<Projectile> {projectile};
            }

            var projectiles = new List<Projectile>();
            foreach (var kvp in ProjectileNamesToIds)
            {
                var name = kvp.Key;
                id = kvp.Value;
                // Check projectile names with CurrentCultureIgnoreCase.
                if (name.Equals(nameOrId, StringComparison.CurrentCultureIgnoreCase))
                {
                    var projectile = new Projectile();
                    projectile.SetDefaults(id);
                    return new List<Projectile> {projectile};
                }
                if (name.StartsWith(nameOrId, StringComparison.CurrentCultureIgnoreCase))
                {
                    var projectile = new Projectile();
                    projectile.SetDefaults(id);
                    projectiles.Add(projectile);
                }
            }
            return projectiles;
        }

        /// <summary>
        ///     Shows an error message.
        /// </summary>
        /// <param name="message">The message, which must not be <c>null</c>.</param>
        public static void ShowErrorMessage([NotNull] string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Main.NewText(message, 255, 0, 0);
        }

        /// <summary>
        ///     Shows an informational message.
        /// </summary>
        /// <param name="message">The message, which must not be <c>null</c>.</param>
        public static void ShowInfoMessage([NotNull] string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Main.NewText(message, 255, 255, 0);
        }

        /// <summary>
        ///     Shows a success message.
        /// </summary>
        /// <param name="message">The message, which must not be <c>null</c>.</param>
        public static void ShowSuccessMessage([NotNull] string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Main.NewText(message, 0, 128, 0);
        }

        /// <summary>
        ///     Shows a warning message.
        /// </summary>
        /// <param name="message">The message, which must not be <c>null</c>.</param>
        public static void ShowWarningMessage([NotNull] string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Main.NewText(message, 255, 69, 0);
        }

        internal static void InitializeNames()
        {
            var item = new Item();
            for (var i = 1; i < Main.maxItemTypes; ++i)
            {
                item.SetDefaults(i);
                ItemNamesToIds[item.Name] = i;
            }
            var projectile = new Projectile();
            for (var i = 1; i < Main.maxProjectileTypes; ++i)
            {
                projectile.SetDefaults(i);
                ProjectileNamesToIds[projectile.Name] = i;
            }
        }
    }
}
