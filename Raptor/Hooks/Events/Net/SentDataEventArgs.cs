using System;
using JetBrains.Annotations;
using Terraria.Localization;

namespace Raptor.Hooks.Events.Net
{
    /// <summary>
    ///     Provides data for the <see cref="NetHooks.SentData" /> event.
    /// </summary>
    [PublicAPI]
    public class SentDataEventArgs : EventArgs
    {
        internal SentDataEventArgs(int packetType, NetworkText text, int number1, float number2, float number3,
            float number4, int number5, int number6, int number7)
        {
            PacketType = (PacketTypes)packetType;
            Text = text;
            Number = number1;
            Number2 = number2;
            Number3 = number3;
            Number4 = number4;
            Number5 = number5;
            Number6 = number6;
            Number7 = number7;
        }

        /// <summary>
        ///     Gets the first argument of the message.
        /// </summary>
        public int Number { get; }

        /// <summary>
        ///     Gets the second argument of the message.
        /// </summary>
        public float Number2 { get; }

        /// <summary>
        ///     Gets the third argument of the message.
        /// </summary>
        public float Number3 { get; }

        /// <summary>
        ///     Gets the fourth argument of the message.
        /// </summary>
        public float Number4 { get; }

        /// <summary>
        ///     Gets the fifth argument of the message.
        /// </summary>
        public int Number5 { get; }

        /// <summary>
        ///     Gets the sixth argument of the message.
        /// </summary>
        public int Number6 { get; }

        /// <summary>
        ///     Gets the seventh argument of the message.
        /// </summary>
        public int Number7 { get; }

        /// <summary>
        ///     Gets the message packet type.
        /// </summary>
        public PacketTypes PacketType { get; }

        /// <summary>
        ///     Gets the message text.
        /// </summary>
        public NetworkText Text { get; }
    }
}
