using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Content;
using Raptor.Hooks.Events.Game;
using Terraria;

namespace Raptor.Hooks
{
    /// <summary>
    ///     Holds the game hooks.
    /// </summary>
    [PublicAPI]
    public static class GameHooks
    {
        /// <summary>
        ///     Invoked when the game is initialized.
        /// </summary>
        public static event EventHandler Initialized;

        /// <summary>
        ///     Invoked when lighting is occurring.
        /// </summary>
        public static event EventHandler<LightingEventArgs> Lighting;

        /// <summary>
        ///     Invoked when the game content is loaded.
        /// </summary>
        public static event EventHandler<LoadedContentEventArgs> LoadedContent;

        /// <summary>
        ///     Invoked when the game is updating.
        /// </summary>
        public static event EventHandler<HandledEventArgs> Update;

        /// <summary>
        ///     Invoked when the game is updated.
        /// </summary>
        public static event EventHandler Updated;

        internal static void InvokeInitialized()
        {
            Initialized?.Invoke(null, EventArgs.Empty);
        }

        internal static bool InvokeLighting(object swipeData)
        {
            var args = new LightingEventArgs((Lighting.LightingSwipeData)swipeData);
            Lighting?.Invoke(null, args);
            return args.Handled;
        }

        internal static void InvokeLoadedContent(ContentManager contentManager)
        {
            LoadedContent?.Invoke(null, new LoadedContentEventArgs(contentManager));
        }

        internal static bool InvokeUpdate()
        {
            var args = new HandledEventArgs();
            Update?.Invoke(null, args);
            return args.Handled;
        }

        internal static void InvokeUpdated()
        {
            Updated?.Invoke(null, EventArgs.Empty);
        }
    }
}
