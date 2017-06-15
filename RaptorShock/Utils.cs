using System;
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
        /// <summary>
        ///     Gets the local player.
        /// </summary>
        [NotNull]
        public static Player LocalPlayer => Main.player[Main.myPlayer];

        /// <summary>
        ///     Gets the local player's selected item.
        /// </summary>
        [NotNull]
        public static Item LocalPlayerItem => LocalPlayer.inventory[LocalPlayer.selectedItem];

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
    }
}
